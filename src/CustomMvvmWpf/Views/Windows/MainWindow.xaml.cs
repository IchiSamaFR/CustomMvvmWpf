using CustomMvvmWpf.ViewModels.Pages;
using CustomMvvmWpf.ViewModels.Windows;
using System.ComponentModel;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace CustomMvvmWpf.Views.Windows;

/// <summary>
/// Fenêtre principale Fluent hébergeant la NavigationView de l'application.
/// </summary>
public partial class MainWindow : FluentWindow, INavigationWindow
{
    public MainWindowVM ViewModel { get; }

    public MainWindow(
        MainWindowVM viewModel,
        HomeVM queryViewModel,
        INavigationViewPageProvider pageProvider,
        INavigationService navigationService,
        ISnackbarService snackbarService,
        IContentDialogService contentDialogService)
    {
        ViewModel = viewModel;
        DataContext = viewModel;

        // Applique le thème système (clair/sombre) et le suit dynamiquement.
        SystemThemeWatcher.Watch(this);

        InitializeComponent();

        SetPageService(pageProvider);
        navigationService.SetNavigationControl(RootNavigation);
        snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        contentDialogService.SetDialogHost(RootContentDialog);

        // Page affichée au démarrage.
        Loaded += (_, _) => _ = Navigate(typeof(Views.Pages.HomePage));
    }

    public INavigationView GetNavigation() => RootNavigation;

    public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

    public void SetPageService(INavigationViewPageProvider navigationViewPageProvider) =>
        RootNavigation.SetPageProviderService(navigationViewPageProvider);

    public void SetServiceProvider(IServiceProvider serviceProvider) =>
        throw new NotSupportedException("Utilisez SetPageService à la place.");

    public void ShowWindow() => Show();

    public void CloseWindow() => Close();
}
