# TP#1 - Hachage de Mots de Passe
## Sécurité Informatique INF36207 - Hiver 2026

### Description
Ce projet contient deux applications console pour le TP#1 sur le hachage de mots de passe :
- **PwdForge** : Générateur de dictionnaires de mots de passe
- **HashProbe** : Cracker de hachages bcrypt par dictionnaire

### Prérequis
- .NET 8.0 SDK ou supérieur
- Visual Studio 2022 ou Visual Studio Code
- Windows 11 (pour l'exécution des .exe)

### Installation

1. Cloner ou télécharger le projet
2. Ouvrir la solution `TP1_HachageMDP.sln` dans Visual Studio
3. Restaurer les packages NuGet :
   ```bash
   dotnet restore
   ```

### Dépendances NuGet

#### HashProbe
- **BCrypt.Net-Next** (version 4.0.3)
  - Installation : `dotnet add package BCrypt.Net-Next`

### Structure du projet

```
TP1_SECURITE_INFORMATIQUE/
├── src/
│   ├── PwdForge/          # Générateur de dictionnaires
│   └── HashProbe/         # Cracker bcrypt
├── data/                  # Dictionnaires générés
└── docs/                  # Documentation et captures
```

### Compilation

```bash
# Compiler toute la solution
dotnet build

# Compiler un projet spécifique
dotnet build src/PwdForge/PwdForge.csproj
dotnet build src/HashProbe/HashProbe.csproj
```

### Exécution

#### PwdForge
```bash
cd src/PwdForge
dotnet run
```

#### HashProbe
```bash
cd src/HashProbe
dotnet run
```

### Utilisation

#### PwdForge - Génération de dictionnaire

1. Lancer l'application
2. Entrer les paramètres :
   - Longueur minimale (ex: 3)
   - Longueur maximale (ex: 6)
   - Caractères permis (ex: abcdefghijklmnopqrstuvwxyz1234567890)
   - Chemin de sortie (ex: ..\..\data\dict_1.txt)
3. Attendre la génération complète
4. Le fichier sera créé dans le répertoire spécifié

#### HashProbe - Cassage de hachage

1. Lancer l'application
2. Entrer le hachage bcrypt à tester (format: $2b$10$...)
3. Entrer le chemin du fichier dictionnaire
4. Observer la progression en temps réel
5. Le résultat s'affichera automatiquement si trouvé

### Exemples de hachages à tester

Les hachages fournis dans le TP :
1. `$2b$10$yFJlKHNYvr2p9lc.CV78..jSMA8txTD1bP6GwpGgvpiR84TQde03O` (longueur 6, chars: a b G M 1 2)
2. `$2b$10$Hh0UhfhtZjrRHdEOxDe3IeYVTnPugoDUtatVDawitU6uTu6tjNDsq` (longueur 6, chars: a r s m 1 2)
3. `$2b$10$yZLhRuimD8WcsBWyvgI5Ku2WE5iugYWbDNviN8MyWUYuk29k8q66O` (longueur 6, chars: a b A B 8 9)
4. `$2b$10$WNSPHl/HRs9PBuB6MPP2UemGxQZoLwGuLsC0TJRxhW7DPwx2vHPqq` (longueur 6, chars: a b c)
5. `$2b$10$FfNcnJ5bqVU7soHsTjH27enW31X0XVL/j9mAAvHW8YXhhBqmXr3iq` (longueur 6, chars: X Y Z)
6-10. Utiliser `dico_fr.txt` fourni

### Notes importantes

- Les applications utilisent le streaming pour gérer les gros dictionnaires
- Le facteur de coût bcrypt est fixé à 10 (comme spécifié dans le TP)
- Les dictionnaires sont en UTF-8, un mot par ligne
- La progression s'affiche automatiquement toutes les 1000 tentatives (HashProbe)

### Auteurs
[À compléter avec les noms des membres de l'équipe]

### Date
Février 2026
