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

        /// <summary>
        /// Initialise le cracker avec le hachage cible et le chemin du dictionnaire
        /// </summary>
        public BcryptCracker(string targetHash, string dictionaryPath)
        {
            _targetHash = targetHash;
            _dictionaryPath = dictionaryPath;
            _attempts = 0;
            _found = false;
        }

        /// <summary>
        /// Valide que le hash fourni est bien un hash bcrypt valide
        /// Format attendu : $2b$10$[22 caractères de sel][31 caractères de hash]
        /// </summary>
        public static bool ValidateBcryptHash(string hash)
        {
            // TODO ÉTAPE 3.1 : Valider le format bcrypt
            // - Vérifier que ça commence par $2b$10$
            // - Vérifier la longueur totale (60 caractères)
            // - Vérifier le format Base64 modifié
            // Retourner true si valide, false sinon

            if (string.IsNullOrWhiteSpace(hash))
                return false;

            // Format bcrypt : $2b$10$[22 chars][31 chars] = 60 caractères total
            if (hash.Length != 60)
                return false;

            if (!hash.StartsWith("$2b$10$"))
                return false;

            // TODO : Valider que le reste est en Base64 modifié (./A-Za-z0-9)
            return true;
        }

        /// <summary>
        /// Compte le nombre de lignes dans le dictionnaire
        /// </summary>
        public long CountDictionaryLines()
        {
            // TODO ÉTAPE 3.2 : Compter les lignes du fichier
            // Utiliser StreamReader pour lire ligne par ligne (streaming)
            // Retourner le nombre total de lignes
            long count = 0;
            // using (var reader = new StreamReader(_dictionaryPath, Encoding.UTF8))
            // {
            //     while (reader.ReadLine() != null)
            //         count++;
            // }
            return count;
        }

        /// <summary>
        /// Lance le processus de cassage du hachage bcrypt
        /// </summary>
        public void Crack()
        {
            _startTime = DateTime.Now;
            _attempts = 0;
            _found = false;

            Console.WriteLine($"\nDémarrage du cassage...");
            Console.WriteLine($"Hachage cible : {_targetHash}");
            Console.WriteLine($"Dictionnaire : {_dictionaryPath}\n");

            // TODO ÉTAPE 3.4 : Lire le dictionnaire ligne par ligne (streaming)
            // using (var reader = new StreamReader(_dictionaryPath, Encoding.UTF8))
            // {
            //     string? password;
            //     while ((password = reader.ReadLine()) != null && !_found)
            //     {
            //         // TODO : Nettoyer le mot de passe (Trim, ignorer les lignes vides)
            //         if (string.IsNullOrWhiteSpace(password))
            //             continue;

            //         // TODO : Tester le mot de passe avec bcrypt
            //         TestPassword(password.Trim());
            //     }
            // }

            // TODO ÉTAPE 3.4 : Afficher le résultat final
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
                // TODO ÉTAPE 3.4 : Utiliser BCrypt.Net pour vérifier le mot de passe
                // bool isValid = BCrypt.Net.BCrypt.Verify(password, _targetHash);
                
                // TODO ÉTAPE 3.4 : Si valide, extraire le sel et arrêter
                // if (isValid)
                // {
                //     _found = true;
                //     _foundPassword = password;
                //     _foundSalt = ExtractSalt(_targetHash);
                //     DisplaySuccess();
                //     return;
                // }

                // TODO ÉTAPE 3.4 : Afficher la progression toutes les X tentatives
                // Exemple : toutes les 1000 tentatives ou toutes les secondes
                if (_attempts % 1000 == 0)
                {
                    DisplayProgress();
                }
            }
            catch (Exception ex)
            {
                // TODO ÉTAPE 3.4 : Gérer les erreurs (log si nécessaire)
                // Ne pas arrêter le processus pour une erreur isolée
            }
        }

        /// <summary>
        /// Extrait le sel du hachage bcrypt
        /// Format : $2b$10$[sel de 22 caractères][hash de 31 caractères]
        /// </summary>
        private string ExtractSalt(string hash)
        {
            // TODO ÉTAPE 3.4 : Extraire le sel
            // Le sel est les caractères 7 à 28 (après "$2b$10$")
            if (hash.Length >= 29)
            {
                return hash.Substring(7, 22); // $2b$10$ = 7 caractères, sel = 22 caractères
            }
            return "";
        }

        /// <summary>
        /// Affiche la progression en temps réel
        /// </summary>
        private void DisplayProgress()
        {
            // TODO ÉTAPE 3.4 : Afficher les statistiques
            // - Nombre de tentatives
            // - Temps écoulé
            // - Taux de tentatives/seconde
            // Utiliser Console.SetCursorPosition pour mettre à jour la même ligne

            var elapsed = DateTime.Now - _startTime;
            var rate = _attempts / Math.Max(elapsed.TotalSeconds, 1);

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write($"Tentatives : {_attempts:N0} | Temps : {elapsed:mm\\:ss} | Vitesse : {rate:F0} essais/sec");
        }

        /// <summary>
        /// Affiche le résultat lorsque le mot de passe est trouvé
        /// </summary>
        private void DisplaySuccess()
        {
            Console.WriteLine("\n\n" + new string('=', 60));
            Console.WriteLine("  MOT DE PASSE TROUVÉ !");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"Mot de passe : {_foundPassword}");
            Console.WriteLine($"Sel utilisé   : {_foundSalt}");
            Console.WriteLine($"Tentatives    : {_attempts:N0}");
            Console.WriteLine($"Temps écoulé  : {DateTime.Now - _startTime:mm\\:ss}");
            Console.WriteLine(new string('=', 60));
        }

        /// <summary>
        /// Affiche le résultat final si le mot de passe n'a pas été trouvé
        /// </summary>
        private void DisplayFinalResult()
        {
            if (!_found)
            {
                var elapsed = DateTime.Now - _startTime;
                Console.WriteLine("\n\n" + new string('=', 60));
                Console.WriteLine("  MOT DE PASSE NON TROUVÉ");
                Console.WriteLine(new string('=', 60));
                Console.WriteLine($"Tentatives totales : {_attempts:N0}");
                Console.WriteLine($"Temps écoulé       : {elapsed:mm\\:ss}");
                Console.WriteLine($"Vitesse moyenne    : {_attempts / Math.Max(elapsed.TotalSeconds, 1):F0} essais/sec");
                Console.WriteLine(new string('=', 60));
            }
        }

        // Propriétés publiques pour accès aux résultats
        public bool Found => _found;
        public string? FoundPassword => _foundPassword;
        public string? FoundSalt => _foundSalt;
        public long Attempts => _attempts;
    }
}
