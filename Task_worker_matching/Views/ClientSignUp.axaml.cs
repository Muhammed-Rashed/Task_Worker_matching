using Avalonia.Controls;
using Avalonia.Interactivity;
using Task_worker_matching.Controllers;

namespace Task_worker_matching.Views
{
    public partial class ClientSignUp : UserControl
    {
        public ClientSignUp()
        {
            InitializeComponent();
        }
        
        private void Login_Click(object? sender, RoutedEventArgs e)
        {
            Navigator.Instance.Navigate(new Login());
        }
    }
}
