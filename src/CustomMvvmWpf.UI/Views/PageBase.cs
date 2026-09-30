using CustomMvvmWpf.UI.ViewModels;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace CustomMvvmWpf.UI.Views;

/// <summary>
/// Page de base fortement typée : reçoit son vue-modèle par injection
/// et l'utilise comme contexte de binding.
/// </summary>
/// <typeparam name="TViewModel">Type du vue-modèle associé à la page.</typeparam>
public abstract class PageBase<TViewModel> : Page, INavigableView<TViewModel>
    where TViewModel : ViewModelBase
{
    protected PageBase(TViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        DataContext = viewModel;
    }

    /// <summary>
    /// Vue-modèle de la page, également défini comme <see cref="FrameworkElement.DataContext"/>.
    /// </summary>
    public TViewModel ViewModel { get; }
}
