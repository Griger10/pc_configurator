using System.Windows;
using PCConfigurator.UI.ViewModels;

namespace PCConfigurator.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel mainViewModel)
    {
        InitializeComponent();
        DataContext = mainViewModel;
    }
}
