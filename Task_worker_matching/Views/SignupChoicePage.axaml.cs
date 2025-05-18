using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Task_worker_matching.Controllers;

namespace Task_worker_matching.Views
{
    public partial class SignupChoicePage : UserControl
    {
        public SignupChoicePage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void ClientButton_Click(object? sender, RoutedEventArgs e)
        {
            Navigator.Instance.Navigate(new ClientSignUp());
        }

        private void WorkerButton_Click(object? sender, RoutedEventArgs e)
        {
            Navigator.Instance.Navigate(new WorkerSignUp());
        }

        private void LoginLinkButton_Click(object? sender, RoutedEventArgs e)
        {
            Navigator.Instance.Navigate(new Login());
        }
    }
}