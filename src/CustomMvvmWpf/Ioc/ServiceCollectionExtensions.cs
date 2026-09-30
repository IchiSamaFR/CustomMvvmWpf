using Microsoft.Extensions.DependencyInjection;
using CustomMvvmWpf.Services;
using CustomMvvmWpf.ViewModels.Pages;
using CustomMvvmWpf.ViewModels.Windows;
using CustomMvvmWpf.Views.Pages;
using CustomMvvmWpf.Views.Windows;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;

namespace CustomMvvmWpf;

/// <summary>
/// Enregistrement centralisé des dépendances de l'application.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services
            .AddWpfUiServices()
            .AddApplicationDomainServices()
            .AddWindows()
            .AddPages();
    }

    private static IServiceCollection AddWpfUiServices(this IServiceCollection services)
    {
        // Fournit les pages de la NavigationView depuis le conteneur.
        _ = services.AddNavigationViewPageProvider();

        _ = services.AddSingleton<INavigationService, NavigationService>();
        _ = services.AddSingleton<ISnackbarService, SnackbarService>();
        _ = services.AddSingleton<IContentDialogService, ContentDialogService>();

        return services;
    }

    private static IServiceCollection AddApplicationDomainServices(this IServiceCollection services)
    {
        // Fichiers de requêtes : lecture/écriture, récents et boîtes de dialogue système.
        _ = services.AddSingleton<FileDialogService>();

        return services;
    }

    private static IServiceCollection AddWindows(this IServiceCollection services)
    {
        _ = services.AddSingleton<MainWindow>();
        _ = services.AddSingleton<MainWindowVM>();

        return services;
    }

    private static IServiceCollection AddPages(this IServiceCollection services)
    {
        _ = services.AddSingleton<HomePage>();
        _ = services.AddSingleton<HomeVM>();

        return services;
    }
}
