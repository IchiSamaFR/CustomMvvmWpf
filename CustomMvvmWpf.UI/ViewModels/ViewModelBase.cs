using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Abstractions.Controls;

namespace CustomMvvmWpf.UI.ViewModels;

/// <summary>
/// Classe de base de tous les vue-modèles : notification de propriétés
/// et prise en charge du cycle de vie de navigation de WPF-UI.
/// </summary>
public abstract partial class ViewModelBase : ObservableObject, INavigationAware
{
    /// <summary>
    /// Indique si <see cref="OnInitialize"/> a déjà été exécuté.
    /// </summary>
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// Appelé une seule fois, à la première navigation vers la vue associée.
    /// </summary>
    protected virtual void OnInitialize()
    {
    }

    /// <inheritdoc />
    public Task OnNavigatedToAsync()
    {
        if (!IsInitialized)
        {
            IsInitialized = true;
            OnInitialize();
        }

        return OnNavigatedTo();
    }

    /// <inheritdoc />
    public Task OnNavigatedFromAsync() => OnNavigatedFrom();

    /// <summary>
    /// Appelé à chaque navigation vers la vue associée.
    /// </summary>
    protected virtual Task OnNavigatedTo() => Task.CompletedTask;

    /// <summary>
    /// Appelé à chaque navigation quittant la vue associée.
    /// </summary>
    protected virtual Task OnNavigatedFrom() => Task.CompletedTask;
}
