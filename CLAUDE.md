# CustomMvvmWpf - Instructions for Claude

## About the project

CustomMvvmWpf is a WPF desktop application (`net8.0-windows`, WPF-UI, MVVM with CommunityToolkit.Mvvm, .NET generic host).

Solution: `src/CustomMvvmWpf.slnx` (`CustomMvvmWpf` + `CustomMvvmWpf.UI`).

---

## Centralized source

**The master file is `.github/instructions.md`.** It takes precedence over this file and over `.github/copilot-instructions.md` in case of divergence: context, architecture, MVVM, DI, libraries, conventions, anti-patterns, validation.

Read it first, and only search via search/bash if information is missing or seems contradicted by the code.

---

## Key points

1. **MVVM** — every VM inherits from `ViewModelBase`, every page from `PageBase<TViewModel>`. `[ObservableProperty]` on **partial properties**, `[RelayCommand]` for commands.
2. **Layering** — Views → ViewModels → Services. No logic or I/O in code-behind or VMs.
3. **DI** — everything in `Ioc/ServiceCollectionExtensions.cs` (`AddWpfUiServices`, `AddApplicationDomainServices`, `AddWindows`, `AddPages`). Constructor injection; `Ioc.GetRequiredService` as a last resort.
4. **Async** — `async` I/O with `RunBusyAsync`; file dialogs through `FileDialogService`.
5. **UI** — Fluent WPF-UI; styles/tokens in `Resources/Styles`, shared components in `CustomMvvmWpf.UI`.

---

## Anti-patterns to avoid

* Business logic, I/O or navigation in code-behind.
* Direct Win32 / dialogs in a VM.
* Bypassing `ViewModelBase` / `PageBase<>`; `[ObservableProperty]` on fields.
* Blocking the UI thread (`.Result`, `.Wait()`, synchronous I/O); swallowing exceptions.
* Hard-coded style values in views.

---

## Language

* **These instructions are in English; everything else stays in French**: code comments, XML documentation (`/// <summary>`), UI texts, commit messages.
* Class/method names in English.

## Code comments

* Succinct: the **why**, never the what; no large blocks.
* `/// <summary>` on non-trivial public types and members.
* Always in French.

---

## Common processes

### Add a page
1. Service(s) in `Services/<Domain>/` → `AddApplicationDomainServices()`.
2. `XxxVM` (`ViewModels/Pages/`) + `XxxPage` (`Views/Pages/`) → `AddPages()`.
3. `NavigationViewItem` in `MainWindowVM.MenuItems`.
4. `dotnet build src/CustomMvvmWpf.slnx`.

### Commits
French format `type: description` (e.g. `feat: ajout de la page Paramètres`), `#<issue>` if an issue exists.

---

**Last updated**: 2026-09-30
**Centralized source**: `.github/instructions.md`
