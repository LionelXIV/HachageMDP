namespace PwdForge
{
    /// <summary>
    /// Application de génération de dictionnaires de mots de passe
    /// TP#1 - Sécurité Informatique INF36207
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            bool continuer = true;

            while (continuer)
            {
                AfficherMenu();
                string choix = Console.ReadLine() ?? "";

                switch (choix)
                {
                    case "1":
                        GererGeneration();
                        break;
                    case "2":
                        GererEstimation();
                        break;
                    case "0":
                        continuer = false;
                        Console.WriteLine("Au revoir !");
                        break;
                    default:
                        Console.WriteLine("Choix invalide. Veuillez réessayer.");
                        break;
                }

                if (continuer)
                {
                    Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                    try
                    {
                        Console.ReadKey();
                        Console.Clear();
                    }
                    catch (InvalidOperationException)
                    {
                        // Console non disponible (entrée redirigée), continuer sans attendre
                        Console.WriteLine();
                    }
                }
            }
        }

        /// <summary>
        /// Affiche le menu principal
        /// </summary>
        static void AfficherMenu()
        {
            Console.WriteLine("--------------------------------");
            Console.WriteLine("PwdForge - Générateur de dictionnaire");
            Console.WriteLine("1) Générer dictionnaire");
            Console.WriteLine("2) Estimer nombre de combinaisons");
            Console.WriteLine("0) Quitter");
            Console.WriteLine("--------------------------------");
            Console.Write("Votre choix : ");
        }

        /// <summary>
        /// Gère l'option 1 : Générer dictionnaire
        /// </summary>
        static void GererGeneration()
        {
            Console.WriteLine("\n=== Génération de dictionnaire ===");
            
            if (DemanderParametres(out int minLength, out int maxLength, out string allowedChars, out string outputFile))
            {
                AfficherResume(minLength, maxLength, allowedChars, outputFile);
                Console.WriteLine("\n[Fonctionnalité de génération à implémenter]");
            }
        }

        /// <summary>
        /// Gère l'option 2 : Estimer nombre de combinaisons
        /// </summary>
        static void GererEstimation()
        {
            Console.WriteLine("\n=== Estimation du nombre de combinaisons ===");
            
            if (DemanderParametres(out int minLength, out int maxLength, out string allowedChars, out string outputFile))
            {
                AfficherResume(minLength, maxLength, allowedChars, outputFile);
                Console.WriteLine("\n[Fonctionnalité d'estimation à implémenter]");
            }
        }

        /// <summary>
        /// Demande les paramètres à l'utilisateur avec validation
        /// </summary>
        static bool DemanderParametres(out int minLength, out int maxLength, out string allowedChars, out string outputFile)
        {
            minLength = 0;
            maxLength = 0;
            allowedChars = "";
            outputFile = "";

            // Demander longueur minimale
            Console.Write("Longueur minimale : ");
            string minInput = Console.ReadLine() ?? "";
            if (!int.TryParse(minInput, out minLength) || minLength <= 0)
            {
                Console.WriteLine("Erreur : La longueur minimale doit être un nombre positif.");
                return false;
            }

            // Demander longueur maximale
            Console.Write("Longueur maximale : ");
            string maxInput = Console.ReadLine() ?? "";
            if (!int.TryParse(maxInput, out maxLength) || maxLength <= 0)
            {
                Console.WriteLine("Erreur : La longueur maximale doit être un nombre positif.");
                return false;
            }

            // Valider que min <= max
            if (minLength > maxLength)
            {
                Console.WriteLine($"Erreur : La longueur minimale ({minLength}) ne peut pas être supérieure à la longueur maximale ({maxLength}).");
                return false;
            }

            // Demander caractères autorisés
            Console.Write("Caractères autorisés : ");
            allowedChars = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(allowedChars))
            {
                Console.WriteLine("Erreur : Vous devez spécifier au moins un caractère autorisé.");
                return false;
            }

            // Demander nom du fichier de sortie
            Console.Write("Nom du fichier de sortie : ");
            outputFile = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(outputFile))
            {
                Console.WriteLine("Erreur : Vous devez spécifier un nom de fichier.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Affiche un résumé des paramètres saisis
        /// </summary>
        static void AfficherResume(int minLength, int maxLength, string allowedChars, string outputFile)
        {
            Console.WriteLine("\n--- Résumé des paramètres ---");
            Console.WriteLine($"Longueur minimale : {minLength}");
            Console.WriteLine($"Longueur maximale : {maxLength}");
            Console.WriteLine($"Caractères autorisés : {allowedChars}");
            Console.WriteLine($"Fichier de sortie : {outputFile}");
        }
    }
}
