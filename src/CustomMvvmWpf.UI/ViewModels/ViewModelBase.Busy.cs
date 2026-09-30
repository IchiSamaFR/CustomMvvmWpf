using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Abstractions.Controls;

namespace CustomMvvmWpf.UI.ViewModels;

// Partie « occupé » de ViewModelBase.
public abstract partial class ViewModelBase : ObservableObject, INavigationAware
{
    // Compteur plutôt que booléen : plusieurs RunBusyAsync peuvent se chevaucher,
    // l'état ne doit retomber qu'à la fin de la dernière opération.
    private int _busyCount;

    /// <summary>
    /// Vrai tant qu'au moins une opération lancée via <see cref="RunBusyAsync"/> est en cours :
    /// pilote les indicateurs d'activité et la désactivation des commandes de la vue associée.
    /// </summary>
    public bool IsBusy => Volatile.Read(ref _busyCount) > 0;

    /// <summary>
    /// Inverse de <see cref="IsBusy"/> : simplifie les liaisons d'activation de la vue.
    /// </summary>
    public bool IsNotBusy => !IsBusy;

    /// <summary>
    /// Exécute <paramref name="operation"/> en marquant le vue-modèle occupé.
    /// Les appels simultanés sont comptés : <see cref="IsBusy"/> ne repasse à faux
    /// qu'à la fin de la dernière opération, y compris en cas d'exception.
    /// </summary>
    protected async Task RunBusyAsync(Func<Task> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);

        if (Interlocked.Increment(ref _busyCount) == 1)
        {
            RaiseBusyChanged();
        }

        try
        {
            await operation();
        }
        finally
        {
            if (Interlocked.Decrement(ref _busyCount) == 0)
            {
                RaiseBusyChanged();
            }
        }
    }

    // Notifie uniquement sur les transitions 0 ↔ 1 ; la valeur transmise est relue
    // pour rester juste si une autre opération a démarré entre-temps.
    private void RaiseBusyChanged()
    {
        OnPropertyChanged(nameof(IsBusy));
        OnPropertyChanged(nameof(IsNotBusy));
        OnBusyChanged(IsBusy);
    }

    /// <summary>
    /// Appelé à chaque changement de <see cref="IsBusy"/> : point d'extension pour
    /// réévaluer les <c>CanExecute</c> des commandes du vue-modèle dérivé.
    /// </summary>
    protected virtual void OnBusyChanged(bool isBusy)
    {
    }
}
