using Microsoft.Extensions.DependencyInjection;

namespace CustomMvvmWpf;

/// <summary>
/// Point d'accès statique au conteneur d'inversion de contrôle de l'application.
/// À privilégier uniquement là où l'injection par constructeur n'est pas possible
/// (code-behind XAML, factories, etc.).
/// </summary>
public static class Ioc
{
    private static IServiceProvider? _services;

    /// <summary>
    /// Conteneur courant.
    /// </summary>
    public static IServiceProvider Services =>
        _services ?? throw new InvalidOperationException(
            "Le conteneur IoC n'a pas été initialisé. Appelez Ioc.Initialize au démarrage.");

    /// <summary>
    /// Indique si le conteneur a été initialisé.
    /// </summary>
    public static bool IsInitialized => _services is not null;

    /// <summary>
    /// Initialise le conteneur. Appelé une seule fois au démarrage de l'application.
    /// </summary>
    public static void Initialize(IServiceProvider services) =>
        _services = services ?? throw new ArgumentNullException(nameof(services));

    /// <summary>
    /// Résout un service obligatoire.
    /// </summary>
    public static T GetRequiredService<T>()
        where T : class => Services.GetRequiredService<T>();

    /// <summary>
    /// Résout un service optionnel.
    /// </summary>
    public static T? GetService<T>()
        where T : class => Services.GetService<T>();

    /// <summary>
    /// Réinitialise le conteneur (arrêt de l'application, tests).
    /// </summary>
    public static void Reset() => _services = null;
}
