using Avalonia.Controls;
using Avalonia.Interactivity;
using Task_worker_matching.Controllers;

namespace Task_worker_matching.Views
{
    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
        }
        
        private void SignupLinkButton_Click(object? sender, RoutedEventArgs e)
        {
            Navigator.Instance.Navigate(new SignupChoicePage());
        }
    }
}
