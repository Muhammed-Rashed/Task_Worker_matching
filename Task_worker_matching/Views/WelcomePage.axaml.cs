using Avalonia.Controls;
using Avalonia.Interactivity;
using Task_worker_matching.Controllers;

namespace Task_worker_matching.Views
{
    public partial class WelcomePage : UserControl
    {
        // Events to notify the MainWindow to switch pages

        public WelcomePage()
        {
            InitializeComponent();
        }

        private void Login_Click(object? sender, RoutedEventArgs e)
        {
            Navigator.Instance.Navigate(new Login());
        }

        private void SignUpChoice_Click(object? sender, RoutedEventArgs e)
        {
            Navigator.Instance.Navigate(new SignupChoicePage());
        }
    }
}