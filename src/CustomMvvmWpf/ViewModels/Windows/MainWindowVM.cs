using CommunityToolkit.Mvvm.ComponentModel;
using CustomMvvmWpf.UI.ViewModels;
using CustomMvvmWpf.Views.Pages;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace CustomMvvmWpf.ViewModels.Windows;

public partial class MainWindowVM : ViewModelBase
{
    [ObservableProperty]
    public partial string ApplicationTitle { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<object> MenuItems { get; set; } = new ObservableCollection<object>
    {
        new NavigationViewItem
        {
            Content = "Accueil",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
            TargetPageType = typeof(HomePage),
            ToolTip = "Page d'accueil",
        },
    };

    [ObservableProperty]
    public partial ObservableCollection<object> FooterMenuItems { get; set; } = new ObservableCollection<object>
    {
        //new NavigationViewItem
        //{
        //    Content = "Paramètres",
        //    Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
        //    TargetPageType = typeof(SettingsPage),
        //    ToolTip = "Configuration de l'application",
        //},
    };
}
