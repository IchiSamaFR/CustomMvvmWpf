using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Abstractions.Controls;

namespace CustomMvvmWpf.UI.ViewModels;

/// <summary>
/// Classe de base de tous les vue-modèles : notification de propriétés
/// et prise en charge du cycle de vie de navigation de WPF-UI.
/// </summary>
public abstract partial class ViewModelBase : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    /// <summary>
    /// Indique si <see cref="OnInitialize"/> a déjà été exécuté.
    /// </summary>
    public bool IsInitialized => _isInitialized;

    /// <summary>
    /// Vrai pendant une opération longue : pilote les indicateurs d'activité
    /// et la désactivation des commandes de la vue associée.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    public partial bool IsBusy { get; set; }

    /// <summary>
    /// Inverse de <see cref="IsBusy"/> : simplifie les liaisons d'activation de la vue.
    /// </summary>
    public bool IsNotBusy => !IsBusy;

    /// <inheritdoc />
    public virtual Task OnNavigatedToAsync()
    {
        if (!_isInitialized)
        {
            _isInitialized = true;
            OnInitialize();
        }

        return OnNavigatedTo();
    }

    /// <inheritdoc />
    public virtual Task OnNavigatedFromAsync() => OnNavigatedFrom();

    /// <summary>
    /// Exécute <paramref name="operation"/> en marquant le vue-modèle occupé,
    /// <see cref="IsBusy"/> étant systématiquement rétabli à la fin.
    /// </summary>
    protected async Task RunBusyAsync(Func<Task> operation)
    {
        IsBusy = true;

        try
        {
            await operation();
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Appelé à chaque changement de <see cref="IsBusy"/> : point d'extension pour
    /// réévaluer les <c>CanExecute</c> des commandes du vue-modèle dérivé.
    /// </summary>
    protected virtual void OnBusyChanged(bool isBusy)
    {
    }

    partial void OnIsBusyChanged(bool value) => OnBusyChanged(value);

    /// <summary>
    /// Appelé une seule fois, à la première navigation vers la vue associée.
    /// </summary>
    protected virtual void OnInitialize()
    {
    }

    /// <summary>
    /// Appelé à chaque navigation vers la vue associée.
    /// </summary>
    protected virtual Task OnNavigatedTo() => Task.CompletedTask;

    /// <summary>
    /// Appelé à chaque navigation quittant la vue associée.
    /// </summary>
    protected virtual Task OnNavigatedFrom() => Task.CompletedTask;
}
