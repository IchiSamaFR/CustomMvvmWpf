# CustomMvvmWpf

Template `dotnet new` de solution **WPF MVVM** prête à l'emploi : WPF-UI (Fluent, thème clair/sombre, Mica), CommunityToolkit.Mvvm et injection de dépendances via le host générique .NET.

| | |
|---|---|
| Package NuGet | `IchiSamaFR.CustomMvvmWpf.Templates` |
| Nom court | `custommvvm` |
| Cible | `net8.0-windows` |
| Flux | GitHub Packages (`https://nuget.pkg.github.com/IchiSamaFR/index.json`) |

---

## Contenu de la solution générée

```
MonApp/
├── MonApp/                  # Application WPF (WinExe)
│   ├── Ioc/                 # Conteneur + enregistrement DI
│   ├── Services/            # FileDialogService + services métier
│   ├── ViewModels/          # Pages/ et Windows/
│   ├── Views/               # Pages/ et Windows/ (NavigationView)
│   └── Resources/Styles/    # Tokens (couleurs, tailles), styles de contrôles
├── MonApp.UI/               # Bibliothèque partagée : ViewModelBase, PageBase<T>,
│                            # layouts, converters, behaviors
├── .github/                 # Instructions pour Copilot / Claude
├── CLAUDE.md
└── MonApp.slnx
```

Dépendances : `CommunityToolkit.Mvvm`, `WPF-UI`, `WPF-UI.DependencyInjection`, `Microsoft.Extensions.Hosting`.

---

## Prérequis

- Windows (WPF ne compile que sous Windows)
- [.NET SDK 8](https://dotnet.microsoft.com/download) ou plus récent — un SDK récent est nécessaire pour le format de solution `.slnx` (SDK 9.0.200+ / 10)
- Visual Studio 2022 17.13+ (optionnel)
- Un compte GitHub (le flux GitHub Packages exige une authentification, même en lecture)

---

## Installation

### 1. Créer un jeton d'accès GitHub

1. GitHub → **Settings → Developer settings → Personal access tokens → Tokens (classic)** → *Generate new token (classic)*.
2. Cocher le scope **`read:packages`** (ajouter `write:packages` uniquement pour publier à la main).
3. Copier le jeton (`ghp_…`).

> Les *fine-grained tokens* ne sont pas supportés par le registre NuGet de GitHub Packages : utiliser un jeton **classic**.

### 2. Ajouter le flux GitHub à NuGet

Une seule fois par machine :

```powershell
dotnet nuget add source "https://nuget.pkg.github.com/IchiSamaFR/index.json" `
  --name github-ichisamafr `
  --username <votre-utilisateur-github> `
  --password <jeton-ghp> `
  --store-password-in-clear-text
```

Vérifier : `dotnet nuget list source`.

> `--store-password-in-clear-text` est requis hors Windows ; sous Windows le mot de passe est chiffré si l'option est omise, mais certaines versions du SDK échouent alors au déchiffrement — garder l'option en cas d'erreur `401`/`Unable to load the service index`.

Le flux apparaît aussi dans Visual Studio (**Outils → Options → Gestionnaire de package NuGet → Sources de package**).

### 3. Installer le template

```powershell
dotnet new install IchiSamaFR.CustomMvvmWpf.Templates
```

Version précise : `dotnet new install IchiSamaFR.CustomMvvmWpf.Templates::1.0.1`

Vérifier : `dotnet new list custommvvm`.

### Alternative : sans flux NuGet

Télécharger le `.nupkg` depuis l'onglet **Releases** du dépôt, puis :

```powershell
dotnet new install .\IchiSamaFR.CustomMvvmWpf.Templates.1.0.1.nupkg
```

---

## Utilisation

### En ligne de commande

```powershell
dotnet new custommvvm -n MonApp
cd MonApp
dotnet build MonApp.slnx
dotnet run --project MonApp
```

- `-n` remplace partout `CustomMvvmWpf` (dossiers, projets, namespaces) par le nom choisi.
- Un dossier `MonApp/` est créé automatiquement ; `-o <chemin>` pour en choisir un autre.

### Dans Visual Studio

**Créer un projet** → rechercher *« WPF MVVM (WPF-UI + DI) »* (filtres : C#, Desktop/WPF).
Si le template n'apparaît pas, redémarrer Visual Studio après l'installation.

---

## Mise à jour et désinstallation

```powershell
dotnet new update --check-only    # voir si une version plus récente existe
dotnet new update                 # mettre à jour les templates installés
dotnet new uninstall IchiSamaFR.CustomMvvmWpf.Templates
```

Les solutions déjà générées ne sont pas affectées par une mise à jour du template.

---

## Publier une nouvelle version (mainteneur)

La publication est automatisée par `.github/workflows/release-template.yml`. Elle se déclenche sur un **push sur `master`** dont le **dernier commit commence par `Release`** :

```powershell
git commit --allow-empty -m "Release 1.1.0"
git push
```

| Message de commit | Version publiée |
|---|---|
| `Release 1.1.0 …` ou `Release v1.1.0` | `1.1.0` |
| `Release 2.0.0-beta.1` | `2.0.0-beta.1` |
| `Release` (sans numéro) | `1.0.<n° de run>` |

Le workflow :
1. packe `template/CustomMvvmWpf.Templates.csproj` avec la version calculée ;
2. teste le template (installation, génération d'une solution `TemplateCheck`, build) ;
3. publie le `.nupkg` sur GitHub Packages (`--skip-duplicate`) ;
4. crée la GitHub Release `v<version>` avec le `.nupkg` en pièce jointe.

> Une version déjà publiée ne peut pas être republiée : incrémenter le numéro.

### Tester le template en local

Depuis la racine du dépôt :

```powershell
# Installation directe depuis le dossier
dotnet new install . --force
dotnet new custommvvm -n Essai -o ..\Essai

# Ou en passant par le package
dotnet pack template/CustomMvvmWpf.Templates.csproj -c Release -o out
dotnet new install .\out\IchiSamaFR.CustomMvvmWpf.Templates.1.0.1.nupkg --force
```

Revenir ensuite à la version du flux : `dotnet new uninstall <chemin ou package>` puis réinstaller.

### Fichiers propres au template

| Fichier | Rôle |
|---|---|
| `.template.config/template.json` | Définition du template (`shortName`, `sourceName`, exclusions) |
| `template/CustomMvvmWpf.Templates.csproj` | Projet de packaging (métadonnées NuGet, version par défaut) |
| `.github/workflows/release-template.yml` | CI de publication |

Ces fichiers sont exclus des solutions générées.

---

## Dépannage

| Symptôme | Cause probable |
|---|---|
| `401 Unauthorized` / `Unable to load the service index` | Jeton expiré, sans `read:packages`, ou fine-grained → recréer un jeton classic et `dotnet nuget update source github-ichisamafr --username … --password …` |
| `No templates or subcommands found matching: 'custommvvm'` | Template non installé → `dotnet new list` |
| Le `.slnx` ne s'ouvre pas | SDK / Visual Studio trop ancien → mettre à jour |
| Erreur de build sur `net8.0-windows` | Compilation hors Windows ou SDK 8 absent |

---

## Licence

MIT
