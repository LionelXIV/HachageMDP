using System.Numerics;
using System.Text;

namespace PwdForge
{
    /// <summary>
    /// Générateur de dictionnaires de mots de passe
    /// Génère toutes les combinaisons possibles selon les paramètres fournis
    /// </summary>
    public class DictionaryGenerator
    {
        private int _minLength;
        private int _maxLength;
        private string _allowedChars;
        private string _outputPath;
        private BigInteger _totalCombinations;
        private BigInteger _currentCount;

        /// <summary>
        /// Initialise le générateur avec les paramètres fournis
        /// </summary>
        public DictionaryGenerator(int minLength, int maxLength, string allowedChars, string outputPath)
        {
            _minLength = minLength;
            _maxLength = maxLength;
            _allowedChars = allowedChars;
            _outputPath = outputPath;
            _currentCount = 0;
            _totalCombinations = EstimateCount(minLength, maxLength, allowedChars.Length);
        }

        /// <summary>
        /// Calcule le nombre total de combinaisons possibles
        /// Formule : Σ(charsetLength^k) pour k de minLen à maxLen
        /// Utilise BigInteger pour gérer les très grands nombres
        /// </summary>
        /// <param name="minLen">Longueur minimale</param>
        /// <param name="maxLen">Longueur maximale</param>
        /// <param name="charsetLength">Nombre de caractères dans le charset</param>
        /// <returns>Nombre total de combinaisons (BigInteger)</returns>
        public static BigInteger EstimateCount(int minLen, int maxLen, int charsetLength)
        {
            BigInteger total = BigInteger.Zero;
            
            for (int k = minLen; k <= maxLen; k++)
            {
                // Calcul de charsetLength^k
                BigInteger power = BigInteger.One;
                for (int i = 0; i < k; i++)
                {
                    power *= charsetLength;
                }
                total += power;
            }
            
            return total;
        }

        /// <summary>
        /// Génère le dictionnaire et l'écrit dans le fichier de sortie
        /// Utilise une génération itérative pour éviter les problèmes de récursion profonde
        /// </summary>
        /// <param name="minLen">Longueur minimale</param>
        /// <param name="maxLen">Longueur maximale</param>
        /// <param name="charset">Caractères autorisés</param>
        /// <param name="outputPath">Chemin du fichier de sortie</param>
        /// <param name="progressEvery">Afficher la progression toutes les X lignes</param>
        public static void GenerateToFile(int minLen, int maxLen, string charset, string outputPath, int progressEvery = 10000)
        {
            // Validation des paramètres
            if (string.IsNullOrEmpty(charset))
            {
                throw new ArgumentException("Le charset ne peut pas être vide.", nameof(charset));
            }

            if (minLen < 1 || maxLen < minLen)
            {
                throw new ArgumentException("Longueurs invalides.", nameof(minLen));
            }

            // Validation du chemin de sortie
            try
            {
                string directory = Path.GetDirectoryName(outputPath) ?? "";
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Impossible de créer le répertoire de sortie : {ex.Message}", ex);
            }

            DateTime startTime = DateTime.Now;
            BigInteger currentCount = BigInteger.Zero;
            BigInteger totalCombinations = EstimateCount(minLen, maxLen, charset.Length);

            Console.WriteLine($"\nGénération en cours...");
            Console.WriteLine($"Total de combinaisons à générer : {totalCombinations:N0}");

            try
            {
                using (var writer = new StreamWriter(outputPath, false, Encoding.UTF8))
                {
                    // Générer pour chaque longueur de minLen à maxLen
                    for (int length = minLen; length <= maxLen; length++)
                    {
                        GenerateForLength(writer, charset, length, ref currentCount, totalCombinations, startTime, progressEvery);
                    }
                }

                TimeSpan elapsed = DateTime.Now - startTime;
                Console.WriteLine($"\n✓ Génération terminée !");
                Console.WriteLine($"  - Mots générés : {currentCount:N0}");
                Console.WriteLine($"  - Fichier : {outputPath}");
                Console.WriteLine($"  - Temps écoulé : {elapsed.TotalSeconds:F2} secondes");
                
                // Estimation de la taille du fichier (approximative)
                if (File.Exists(outputPath))
                {
                    long fileSize = new FileInfo(outputPath).Length;
                    Console.WriteLine($"  - Taille du fichier : {fileSize:N0} octets ({fileSize / 1024.0:F2} KB)");
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new IOException($"Accès refusé au fichier : {outputPath}");
            }
            catch (DirectoryNotFoundException)
            {
                throw new IOException($"Répertoire introuvable : {Path.GetDirectoryName(outputPath)}");
            }
            catch (IOException ex)
            {
                throw new IOException($"Erreur d'écriture dans le fichier : {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Génère toutes les combinaisons d'une longueur donnée de manière itérative
        /// Utilise un tableau d'indices pour éviter la récursion profonde
        /// </summary>
        private static void GenerateForLength(StreamWriter writer, string charset, int length, 
            ref BigInteger currentCount, BigInteger totalCombinations, DateTime startTime, int progressEvery)
        {
            int charsetLength = charset.Length;
            
            // Tableau d'indices pour représenter la position actuelle dans chaque position
            int[] indices = new int[length];
            
            // Initialiser tous les indices à 0
            for (int i = 0; i < length; i++)
            {
                indices[i] = 0;
            }

            bool finished = false;
            
            while (!finished)
            {
                // Construire le mot de passe à partir des indices actuels
                StringBuilder word = new StringBuilder(length);
                for (int i = 0; i < length; i++)
                {
                    word.Append(charset[indices[i]]);
                }
                
                // Écrire le mot dans le fichier
                writer.WriteLine(word.ToString());
                currentCount++;
                
                // Afficher la progression si nécessaire
                if (currentCount % progressEvery == 0 || currentCount == totalCombinations)
                {
                    DisplayProgress(currentCount, totalCombinations, startTime);
                }
                
                // Passer à la combinaison suivante
                // Incrémenter les indices de droite à gauche (comme un compteur)
                int position = length - 1;
                while (position >= 0)
                {
                    indices[position]++;
                    if (indices[position] < charsetLength)
                    {
                        // Pas de débordement, on continue
                        break;
                    }
                    else
                    {
                        // Débordement : remettre à 0 et passer à la position précédente
                        indices[position] = 0;
                        position--;
                    }
                }
                
                // Si on a débordé toutes les positions, on a terminé
                if (position < 0)
                {
                    finished = true;
                }
            }
        }

        /// <summary>
        /// Affiche la progression de la génération
        /// </summary>
        private static void DisplayProgress(BigInteger currentCount, BigInteger totalCombinations, DateTime startTime)
        {
            TimeSpan elapsed = DateTime.Now - startTime;
            double percentage = 0;
            
            if (totalCombinations > 0)
            {
                // Calcul du pourcentage (approximatif pour BigInteger)
                percentage = (double)(currentCount * 100) / (double)totalCombinations;
            }
            
            double speed = 0;
            if (elapsed.TotalSeconds > 0)
            {
                speed = (double)currentCount / elapsed.TotalSeconds;
            }
            
            Console.Write($"\rProgression : {currentCount:N0} / {totalCombinations:N0} ({percentage:F2}%) | " +
                         $"Temps : {elapsed.TotalSeconds:F1}s | " +
                         $"Vitesse : {speed:N0} mots/s");
        }

        /// <summary>
        /// Retourne le nombre total de combinaisons calculées
        /// </summary>
        public BigInteger TotalCombinations => _totalCombinations;
    }
}
