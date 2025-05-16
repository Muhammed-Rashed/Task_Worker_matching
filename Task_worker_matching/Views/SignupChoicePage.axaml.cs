using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Task_worker_matching
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

            this.FindControl<Button>("ClientButton").Click += ClientSignup_Click;
            this.FindControl<Button>("WorkerButton").Click += WorkerSignup_Click;
            this.FindControl<TextBlock>("LoginLink").PointerPressed += LoginLink_Click;
        }

        private void ClientSignup_Click(object? sender, RoutedEventArgs e)
        {
            ShowMessage("Client signup clicked!");
        }

        private void WorkerSignup_Click(object? sender, RoutedEventArgs e)
        {
            ShowMessage("Worker signup clicked!");
        }

        private void LoginLink_Click(object? sender, PointerPressedEventArgs e)
        {
            ShowMessage("Navigate to login page.");
        }

        private async void ShowMessage(string message)
        {
            var dialog = new Window
            {
                Width = 300,
                Height = 150,
                Content = new StackPanel
                {
                    Margin = new Thickness(20),
                    Children =
                    {
                        new TextBlock { Text = message, Margin = new Thickness(0, 0, 0, 20) },
                        new Button
                        {
                            Content = "OK",
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            Width = 80
                        }
                    }
                }
            };

            var okButton = ((StackPanel)dialog.Content).Children[1] as Button;
            okButton.Click += (_, _) => dialog.Close();
        }
    }
}
