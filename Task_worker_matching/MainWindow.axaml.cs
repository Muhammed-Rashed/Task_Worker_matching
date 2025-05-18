using Avalonia.Controls;
using Task_worker_matching.Controllers;
using Task_worker_matching.Views;

namespace Task_worker_matching;

public partial class MainWindow : Window
{
    private readonly NavbarController _navbarController = new();

    public MainWindow()
    {
        InitializeComponent();

        Navigator.Instance.SetContentControl(MainContent);
        // Show login first
        ShowWelcome();
    }

    private void ShowWelcome()
    {
        var welcomePage = new Views.WelcomePage();
        
        MainContent.Content = welcomePage;
    }
}