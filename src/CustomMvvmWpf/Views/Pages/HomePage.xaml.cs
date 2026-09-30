using CustomMvvmWpf.UI.Views;
using CustomMvvmWpf.ViewModels.Pages;

namespace CustomMvvmWpf.Views.Pages;

public partial class HomePage : PageBase<HomeVM>
{
    public HomePage(HomeVM viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
