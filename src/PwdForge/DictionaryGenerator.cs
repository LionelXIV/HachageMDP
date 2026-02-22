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
        private long _totalCombinations;
        private long _currentCount;
        private DateTime _startTime;

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
            _totalCombinations = CalculateTotalCombinations();
        }

        /// <summary>
        /// Calcule le nombre total de combinaisons possibles
        /// Formule : Σ(n^k) pour k de minLength à maxLength
        /// où n = nombre de caractères permis
        /// </summary>
        private long CalculateTotalCombinations()
        {
            // TODO ÉTAPE 2.3 : Implémenter le calcul
            // Exemple : min=1, max=3, chars="abc" → 3 + 9 + 27 = 39
            long total = 0;
            int n = _allowedChars.Length;
            
            for (int k = _minLength; k <= _maxLength; k++)
            {
                // TODO : Calculer n^k et l'ajouter à total
            }
            
            return total;
        }

        /// <summary>
        /// Génère le dictionnaire et l'écrit dans le fichier de sortie
        /// Utilise le streaming pour éviter de charger tout en mémoire
        /// </summary>
        public void Generate()
        {
            _startTime = DateTime.Now;
            _currentCount = 0;

            // TODO ÉTAPE 2.4 : Ouvrir un StreamWriter vers le fichier de sortie
            // Utiliser UTF-8 encoding
            // using (var writer = new StreamWriter(_outputPath, false, Encoding.UTF8))

            // TODO ÉTAPE 2.4 : Appeler GenerateRecursive() pour chaque longueur
            // for (int length = _minLength; length <= _maxLength; length++)
            // {
            //     GenerateRecursive(writer, "", length);
            // }

            // TODO ÉTAPE 2.4 : Afficher le résumé final
        }

        /// <summary>
        /// Génère récursivement toutes les combinaisons de longueur donnée
        /// </summary>
        /// <param name="writer">StreamWriter pour écrire dans le fichier</param>
        /// <param name="current">Combinaison actuelle en construction</param>
        /// <param name="remainingLength">Nombre de caractères restants à ajouter</param>
        private void GenerateRecursive(StreamWriter writer, string current, int remainingLength)
        {
            // TODO ÉTAPE 2.4 : Implémenter la récursion
            // Si remainingLength == 0 :
            //   - Écrire current dans le fichier (une ligne)
            //   - Incrémenter _currentCount
            //   - Afficher la progression toutes les X tentatives (ex: 10000)
            // Sinon :
            //   - Pour chaque caractère dans _allowedChars :
            //     - Appeler récursivement avec current + char, remainingLength - 1
        }

        /// <summary>
        /// Affiche la progression de la génération
        /// </summary>
        private void DisplayProgress()
        {
            // TODO ÉTAPE 2.4 : Afficher la progression
            // - Pourcentage complété
            // - Nombre de mots générés / total
            // - Temps écoulé
            // - Vitesse (mots/seconde)
            // Utiliser Console.SetCursorPosition pour mettre à jour la même ligne
        }

        /// <summary>
        /// Retourne le nombre total de combinaisons calculées
        /// </summary>
        public long TotalCombinations => _totalCombinations;
    }
}
