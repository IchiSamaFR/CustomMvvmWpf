# CustomMvvmWpf - Shared Instructions

**Master file for Claude and Copilot. Read it first.** `CLAUDE.md` and `.github/copilot-instructions.md` are only summaries of it: if they diverge, this file wins.

---

## About the project

CustomMvvmWpf is a Windows desktop application (WPF, `net8.0-windows`) built on a reusable MVVM template.

> **TODO (project-specific)**: describe here the purpose of the application, its features and any absolute rule (security, privacy, offline…). Keep the rest of this file generic.

* Solution: `src/CustomMvvmWpf.slnx` (projects `CustomMvvmWpf` and `CustomMvvmWpf.UI`).
* MVVM architecture based on CommunityToolkit.Mvvm, Fluent UI with WPF-UI (`NavigationView`, system light/dark theme, Mica).
* .NET generic host (`Microsoft.Extensions.Hosting`): DI, configuration.

---

## Architecture

### Layers

**`CustomMvvmWpf`** (application, `WinExe`):
* **Views**: `Views/Windows` (`MainWindow`, host of the `NavigationView`) and `Views/Pages` (one XAML page per feature). UI only.
* **ViewModels**: `ViewModels/Windows` and `ViewModels/Pages`, same split as the views. One page = one VM (`XxxPage` ↔ `XxxVM`).
* **Services**: application logic, grouped by domain in `Services/<Domain>/`.
* **Ioc**: `Ioc.cs` (static access to the container) and `ServiceCollectionExtensions.cs` (DI registration).
* **Resources/Styles**: XAML dictionaries — `Tokens/` (colors, font sizes), `Controls/` (per-control styles), `Converter.xaml`, aggregated by `Style.xaml`.

**`CustomMvvmWpf.UI`** (shared WPF component library, no application logic):
* `ViewModels/ViewModelBase`: base of all VMs.
* `Views/PageBase<TViewModel>`: base of all pages.
* `Controls/Layouts`: `GridExtended`, `StackPanelExtended`.
* `Converters/`: `BooleanToVisibilityConverter`, `IsAnyListToBooleanConverter`, `IsAnyListToVisibilityConverter`.
* `Behaviors/`: `DataGridBehavior`.

**Important**: Views → ViewModels → Services. VMs know neither Win32 nor controls; services know nothing about VMs.

### Repository structure

```
CustomMvvmWpf/
├── src/
│   ├── CustomMvvmWpf/
│   │   ├── Ioc/                     # Ioc.cs, ServiceCollectionExtensions.cs
│   │   ├── Services/                # FileDialogService + per-domain services
│   │   ├── ViewModels/{Pages,Windows}/
│   │   ├── Views/{Pages,Windows}/
│   │   ├── Resources/Styles/        # Tokens, Controls, Converter, Style
│   │   └── App.xaml(.cs)
│   ├── CustomMvvmWpf.UI/            # VM/Page bases, layouts, converters, behaviors
│   └── CustomMvvmWpf.slnx
├── .github/                         # instructions.md (master), copilot-instructions.md
└── CLAUDE.md
```

### Existing services

| Service | Role |
|---|---|
| `FileDialogService` | System "Open" / "Save as" dialogs (`PickOpenFile`, `PickSaveFile`); keeps Win32 out of VMs. **Reuse it for any file selection.** |
| `INavigationService`, `ISnackbarService`, `IContentDialogService` | WPF-UI services (navigation, notifications, dialogs), registered as singletons |

---

## MVVM pattern

* All VMs inherit from `ViewModelBase` (`ObservableObject` + `INavigationAware`), all pages from `PageBase<TViewModel>` (VM injected by constructor, set as `DataContext`).
* Lifecycle: `OnInitialize()` (once), `OnNavigatedTo()` / `OnNavigatedFrom()` (on each navigation). Call `base` in overrides.
* Long operations: `RunBusyAsync(...)` (drives `IsBusy` / `IsNotBusy`); `OnBusyChanged` to re-evaluate `CanExecute`.
* Bindable properties: `[ObservableProperty]` on **partial properties** (`public partial string Title { get; set; }`), never on fields.
* Commands: `[RelayCommand]` (asynchronous for any I/O), with `CanExecute` bound to `IsNotBusy`.
* Page code-behind: constructor + `InitializeComponent()` only.

---

## Dependency injection

Everything is registered in `Ioc/ServiceCollectionExtensions.cs` (`AddApplicationServices`):

* `AddWpfUiServices()` — WPF-UI services and the `NavigationView` page provider.
* `AddApplicationDomainServices()` — application services (singletons by default).
* `AddWindows()` — `MainWindow` and `MainWindowVM`.
* `AddPages()` — pages and VMs.

Rules: constructor injection; `Ioc.GetRequiredService<T>()` only where it is impossible (code-behind, factories). Existing style is `_ = services.AddSingleton<...>();`.

### Adding a page (procedure)

1. Service(s) in `Services/<Domain>/` (interface if useful for tests), registered in `AddApplicationDomainServices()`.
2. VM in `ViewModels/Pages/` (`XxxVM : ViewModelBase`) and page in `Views/Pages/` (`XxxPage : PageBase<XxxVM>`), registered in `AddPages()`.
3. `NavigationViewItem` entry (`SymbolRegular` icon, `TargetPageType`, `ToolTip`) in `MainWindowVM.MenuItems`.
4. Reuse styles from `Resources/Styles`; converters and layouts from `CustomMvvmWpf.UI`.
5. Build (`dotnet build src/CustomMvvmWpf.slnx`).

---

## Libraries

* **CommunityToolkit.Mvvm** — MVVM.
* **WPF-UI** (+ `WPF-UI.DependencyInjection`) — Fluent UI.
* **Microsoft.Extensions.Hosting** — host, DI, configuration.

Prefer the BCL before adding a dependency. Any new dependency: compatible license, maintained, justified by a real need.

---

## Coding standards

### General principles

* **Target**: `net8.0-windows`, WPF, `Nullable` enabled, `ImplicitUsings`, preview `LangVersion`.
* C# for logic, XAML for UI. Clear code, consistent with the existing one.
* **Error handling**: targeted exceptions, never swallowed; user-facing errors displayed via `ISnackbarService` / `IContentDialogService`.
* **async/await** for all I/O; never `.Result` / `.Wait()`; cancellation support for long operations.
* Dispose resources (`using`, `await using`); prefer streams for large data.
* Magic numbers: constants; one class per file.

### Naming

* Classes / interfaces / methods / properties: PascalCase; interfaces prefixed `I`; `Async` suffix on asynchronous methods.
* Private fields: `_camelCase`; parameters and locals: camelCase; constants: PascalCase.
* Pages: `Page` suffix; ViewModels: `VM` suffix; services: explicit business name + `Service` suffix.

### Style

* Allman braces, **4 spaces** per level, file-scoped namespaces, `using` outside the namespace, unused usings removed.
* Always use braces, even for a one-line `if`.
* Reasonable line length (≈120 characters max).

### Language

* **These instructions (markdown files) are written in English.**
* **Everything else stays in French**: code comments, XML documentation (`/// <summary>` etc.), UI texts and commit messages.
* Class/method names: English.
* Comments: only the *why*, short; `/// <summary>` on non-trivial public types and members.
* Commit format: `type: description` (e.g. `feat: ajout de la page Paramètres`). Add `#<issue>` if an issue exists.

---

## Implementation guidelines

* Simple, pragmatic solutions; no over-engineering.
* Production-ready code, no partial implementation.
* Reuse before creating: `FileDialogService`, `Resources/Styles` styles, `CustomMvvmWpf.UI` components.
* Styles/colors/sizes: through tokens (`Tokens/Color.xaml`, `Tokens/FontSize.xaml`), no hard-coded values in views.
* One service = one responsibility; logic goes neither in the VM nor in the code-behind.
* Generic, reusable components go in `CustomMvvmWpf.UI`; application-specific ones stay in `CustomMvvmWpf`.

---

## Tests

No test project by default. If tests are added: a dedicated `CustomMvvmWpf.UnitTest` project covering services and VMs; do not test XAML code-behind.

---

## Anti-patterns to avoid

* Business logic, I/O or navigation in page code-behind.
* Direct Win32 calls / dialogs in a VM (go through `FileDialogService`).
* Bypassing `ViewModelBase` / `PageBase<>`.
* `[ObservableProperty]` on fields; blocking the UI thread (synchronous I/O, `.Result`).
* Swallowing an exception; duplicated services.
* Hard-coded style values in XAML.

---

## Validation

Build after any change:

```bash
dotnet build src/CustomMvvmWpf.slnx
```

---

**Last updated**: 2026-09-30
