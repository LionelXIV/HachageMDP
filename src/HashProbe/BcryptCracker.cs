using System.Text;
using BCrypt.Net;

namespace HashProbe
{
    /// <summary>
    /// Classe pour casser les hachages bcrypt en utilisant un dictionnaire
    /// </summary>
    public class BcryptCracker
    {
        private string _targetHash;
        private string _dictionaryPath;
        private long _attempts;
        private DateTime _startTime;
        private bool _found;
        private string? _foundPassword;
        private string? _foundSalt;
        private bool _userCancelled;

        /// <summary>
        /// Initialise le cracker avec le hachage cible et le chemin du dictionnaire
        /// </summary>
        public BcryptCracker(string targetHash, string dictionaryPath)
        {
            _targetHash = targetHash;
            _dictionaryPath = dictionaryPath;
            _attempts = 0;
            _found = false;
            _userCancelled = false;
        }

        /// <summary>
        /// Valide que le hash fourni est bien un hash bcrypt valide
        /// Accepte les formats : $2a$, $2b$, $2y$ avec coût 10
        /// Format attendu : $2x$10$[22 caractères de sel][31 caractères de hash] = 60 caractères total
        /// </summary>
        public static bool ValidateBcryptHash(string hash, out string errorMessage)
        {
            errorMessage = "";

            if (string.IsNullOrWhiteSpace(hash))
            {
                errorMessage = "Le hash ne peut pas être vide.";
                return false;
            }

            // Vérifier la longueur totale (60 caractères pour bcrypt)
            if (hash.Length != 60)
            {
                errorMessage = $"Hash invalide : longueur attendue 60 caractères, reçue {hash.Length}.";
                return false;
            }

            // Vérifier le format : doit commencer par $2a$, $2b$, ou $2y$
            if (!hash.StartsWith("$2a$10$") && !hash.StartsWith("$2b$10$") && !hash.StartsWith("$2y$10$"))
            {
                errorMessage = "Hash invalide : doit commencer par $2a$10$, $2b$10$, ou $2y$10$.";
                return false;
            }

            // Vérifier que le coût est bien 10
            if (!hash.Substring(4, 2).Equals("10"))
            {
                errorMessage = "Hash invalide : le coût (cost factor) doit être 10.";
                return false;
            }

            // Vérifier que le reste est en Base64 modifié (./A-Za-z0-9)
            string rest = hash.Substring(7); // Après "$2x$10$"
            foreach (char c in rest)
            {
                if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || 
                      (c >= '0' && c <= '9') || c == '.' || c == '/'))
                {
                    errorMessage = "Hash invalide : contient des caractères non autorisés.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Lance le processus de cassage du hachage bcrypt
        /// </summary>
        public void Crack()
        {
            _startTime = DateTime.Now;
            _attempts = 0;
            _found = false;
            _userCancelled = false;

            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("  DÉMARRAGE DU CASSAGE");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"Fichier dictionnaire : {_dictionaryPath}");
            Console.WriteLine($"Hachage cible        : {_targetHash}");
            Console.WriteLine($"Heure de départ      : {_startTime:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("\nAppuyez sur 'q' pour arrêter à tout moment...\n");

            try
            {
                using (var reader = new StreamReader(_dictionaryPath, Encoding.UTF8))
                {
                    string? password;
                    while ((password = reader.ReadLine()) != null && !_found && !_userCancelled)
                    {
                        // Vérifier si l'utilisateur veut arrêter
                        try
                        {
                            if (Console.KeyAvailable)
                            {
                                var key = Console.ReadKey(true);
                                if (key.KeyChar == 'q' || key.KeyChar == 'Q')
                                {
                                    _userCancelled = true;
                                    Console.WriteLine("\n\nArrêt demandé par l'utilisateur...");
                                    break;
                                }
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            // Console non disponible (entrée redirigée), continuer sans vérification
                        }

                        // Ignorer les lignes vides
                        if (string.IsNullOrWhiteSpace(password))
                            continue;

                        // Tester le mot de passe
                        TestPassword(password.Trim());
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"\n✗ Erreur : Fichier dictionnaire introuvable : {_dictionaryPath}");
                return;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"\n✗ Erreur : Accès refusé au fichier : {_dictionaryPath}");
                return;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"\n✗ Erreur de lecture du fichier : {ex.Message}");
                return;
            }

            // Afficher le résultat final
            DisplayFinalResult();
        }

        /// <summary>
        /// Teste un mot de passe contre le hachage cible
        /// </summary>
        private void TestPassword(string password)
        {
            _attempts++;

            try
            {
                // Utiliser BCrypt.Net pour vérifier le mot de passe
                bool isValid = BCrypt.Net.BCrypt.Verify(password, _targetHash);

                if (isValid)
                {
                    _found = true;
                    _foundPassword = password;
                    _foundSalt = ExtractSalt(_targetHash);
                    DisplaySuccess();
                    return;
                }

                // Afficher la progression toutes les 250 tentatives
                if (_attempts % 250 == 0)
                {
                    DisplayProgress(password);
                }
            }
            catch (Exception)
            {
                // Ne pas arrêter le processus pour une erreur isolée
                // Juste ignorer et continuer
            }
        }

        /// <summary>
        /// Extrait le sel du hachage bcrypt
        /// Format : $2x$10$[sel de 22 caractères][hash de 31 caractères]
        /// </summary>
        private string ExtractSalt(string hash)
        {
            // Le sel est les caractères 7 à 28 (après "$2x$10$")
            if (hash.Length >= 29)
            {
                return hash.Substring(7, 22);
            }
            return "";
        }

        /// <summary>
        /// Affiche la progression en temps réel
        /// </summary>
        private void DisplayProgress(string lastPassword)
        {
            var elapsed = DateTime.Now - _startTime;
            double rate = 0;
            if (elapsed.TotalSeconds > 0)
            {
                rate = _attempts / elapsed.TotalSeconds;
            }

            // Tronquer le dernier mot de passe s'il est trop long
            string displayPassword = lastPassword;
            if (displayPassword.Length > 30)
            {
                displayPassword = displayPassword.Substring(0, 27) + "...";
            }

            // Sauvegarder la position du curseur
            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;

            // Afficher sur la même ligne
            Console.Write($"\rTentatives : {_attempts:N0} | Temps : {elapsed:mm\\:ss} | " +
                         $"Vitesse : {rate:F0} essais/sec | Dernier : {displayPassword}");

            // Restaurer la position du curseur (pour permettre l'affichage sur la même ligne)
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }

        /// <summary>
        /// Affiche le résultat lorsque le mot de passe est trouvé
        /// </summary>
        private void DisplaySuccess()
        {
            var elapsed = DateTime.Now - _startTime;
            
            Console.WriteLine("\n\n" + new string('=', 60));
            Console.WriteLine("  ✓ MOT DE PASSE TROUVÉ !");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"Mot de passe : {_foundPassword}");
            Console.WriteLine($"Sel utilisé   : {_foundSalt}");
            Console.WriteLine($"Tentatives    : {_attempts:N0}");
            Console.WriteLine($"Temps écoulé  : {elapsed:mm\\:ss}");
            Console.WriteLine(new string('=', 60));
        }

        /// <summary>
        /// Affiche le résultat final si le mot de passe n'a pas été trouvé ou si arrêté
        /// </summary>
        private void DisplayFinalResult()
        {
            var elapsed = DateTime.Now - _startTime;
            double avgRate = 0;
            if (elapsed.TotalSeconds > 0)
            {
                avgRate = _attempts / elapsed.TotalSeconds;
            }

            Console.WriteLine("\n\n" + new string('=', 60));
            if (_userCancelled)
            {
                Console.WriteLine("  ⚠ ARRÊT DEMANDÉ PAR L'UTILISATEUR");
            }
            else if (!_found)
            {
                Console.WriteLine("  ✗ MOT DE PASSE NON TROUVÉ");
            }
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"Tentatives totales : {_attempts:N0}");
            Console.WriteLine($"Temps écoulé       : {elapsed:mm\\:ss}");
            Console.WriteLine($"Vitesse moyenne    : {avgRate:F0} essais/sec");
            Console.WriteLine(new string('=', 60));
        }

        // Propriétés publiques pour accès aux résultats
        public bool Found => _found;
        public string? FoundPassword => _foundPassword;
        public string? FoundSalt => _foundSalt;
        public long Attempts => _attempts;
        public bool UserCancelled => _userCancelled;
    }
}
