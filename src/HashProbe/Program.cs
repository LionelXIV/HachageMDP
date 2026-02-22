using BCrypt.Net;

namespace HashProbe
{
    /// <summary>
    /// Application de cassage de hachages bcrypt
    /// TP#1 - Sécurité Informatique INF36207
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            bool continuer = true;

            while (continuer)
            {
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("  HashProbe - Cracker de hachages bcrypt");
                Console.WriteLine(new string('=', 60));
                Console.WriteLine("1) Casser un hachage bcrypt");
                Console.WriteLine("0) Quitter");
                Console.WriteLine(new string('=', 60));
                Console.Write("Votre choix : ");

                string choix = Console.ReadLine() ?? "";

                switch (choix)
                {
                    case "1":
                        GererCassage();
                        break;
                    case "9":
                        GenererHashTest();
                        break;
                    case "0":
                        continuer = false;
                        Console.WriteLine("\nAu revoir !");
                        break;
                    default:
                        Console.WriteLine("\nChoix invalide. Veuillez réessayer.");
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
        /// Gère le processus de cassage d'un hachage bcrypt
        /// </summary>
        static void GererCassage()
        {
            Console.WriteLine("\n=== Cassage de hachage bcrypt ===");

            // Demander le hachage bcrypt
            Console.Write("\nEntrez le hachage bcrypt à casser : ");
            string hash = Console.ReadLine() ?? "";

            // Valider le hachage
            if (!BcryptCracker.ValidateBcryptHash(hash, out string errorMessage))
            {
                Console.WriteLine($"\n✗ Erreur de validation : {errorMessage}");
                Console.WriteLine("\nFormat attendu : $2a$10$... ou $2b$10$... ou $2y$10$...");
                Console.WriteLine("Le coût (cost factor) doit être 10.");
                return;
            }

            Console.WriteLine("✓ Hash valide");

            // Demander le chemin du dictionnaire
            Console.Write("\nEntrez le chemin du fichier dictionnaire : ");
            string dictionaryPath = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(dictionaryPath))
            {
                Console.WriteLine("\n✗ Erreur : Le chemin du dictionnaire ne peut pas être vide.");
                return;
            }

            // Vérifier que le fichier existe
            if (!File.Exists(dictionaryPath))
            {
                Console.WriteLine($"\n✗ Erreur : Le fichier dictionnaire n'existe pas : {dictionaryPath}");
                return;
            }

            Console.WriteLine("✓ Fichier dictionnaire trouvé");

            // Créer le cracker et lancer le cassage
            var cracker = new BcryptCracker(hash, dictionaryPath);
            cracker.Crack();
        }

        /// <summary>
        /// Génère un hash bcrypt de test (TEMPORAIRE - pour tests uniquement)
        /// </summary>
        static void GenererHashTest()
        {
            Console.WriteLine("\n=== Génération de hash bcrypt de test ===");
            Console.Write("\nEntrez le mot de passe pour générer le hash : ");
            string password = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("\n✗ Erreur : Le mot de passe ne peut pas être vide.");
                return;
            }

            try
            {
                string hash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(10));
                
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("  Hash généré (coût 10)");
                Console.WriteLine(new string('=', 60));
                Console.WriteLine($"Hash : {hash}");
                Console.WriteLine(new string('=', 60));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Erreur lors de la génération : {ex.Message}");
            }
        }
    }
}
