using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using CustomMvvmWpf.Views.Windows;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace CustomMvvmWpf;

/// <summary>
/// Point d'entrée de l'application : configure le hôte générique
/// (conteneur IoC, configuration, logs) et affiche la fenêtre principale.
/// </summary>
public partial class App : Application
{
    private static readonly IHost AppHost = Microsoft.Extensions.Hosting.Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(c =>
            c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!))
        .ConfigureServices((_, services) => services.AddApplicationServices())
        .Build();

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        await AppHost.StartAsync();

        Ioc.Initialize(AppHost.Services);

        Ioc.GetRequiredService<MainWindow>().Show();
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        Ioc.Reset();

        await AppHost.StopAsync();
        AppHost.Dispose();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // TODO : journaliser l'exception avant de la remonter.
    }
}
