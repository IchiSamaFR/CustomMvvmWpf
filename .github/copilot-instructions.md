# Copilot Instructions — CustomMvvmWpf

**Master file**: `.github/instructions.md` — it wins in case of divergence. Read it first for the full, up-to-date context.

This file is only a summary. Copilot must always:

* Read the master instructions first and only search via search/bash if information is missing or unexpected.
* Follow MVVM: `ViewModelBase` / `PageBase<TViewModel>`, `[ObservableProperty]` on partial properties, `[RelayCommand]`.
* Put no business logic or I/O in code-behind or VMs: go through services (`Services/<Domain>/`) and `FileDialogService`.
* Register services, VMs and pages in `Ioc/ServiceCollectionExtensions.cs`.
* Use `async`/`await` (never `.Result`) and never swallow exceptions.
* Write code comments, XML documentation (`/// <summary>`), UI texts and commit messages in **French**; only the markdown instructions are in English.

See `.github/instructions.md` for:
- Project context
- Architecture, layers and repository structure
- MVVM pattern, dependency injection, "Adding a page" procedure
- Libraries, coding standards and naming
- Anti-patterns, tests and validation (`dotnet build src/CustomMvvmWpf.slnx`)

---

**Last updated**: 2026-09-30
