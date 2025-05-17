using Avalonia.Controls;
using Avalonia.Media;
using System.Collections.Generic;
using Avalonia;

namespace MyAvaloniaApp.Controllers
{
    public class NavbarController
    {
        private readonly Dictionary<string, Button> _navButtons = new();
        private readonly IBrush _selectedColor = new SolidColorBrush(Color.Parse("#2563eb"));
        private readonly IBrush _defaultColor = new SolidColorBrush(Color.Parse("#1f2937"));
        private readonly IBrush _white = Brushes.White;

        public void Init(StackPanel navbarPanel)
        {
            foreach (var child in navbarPanel.Children)
            {
                if (child is Button button)
                {
                    _navButtons[button.Content?.ToString() ?? ""] = button;

                    button.Click += (_, _) => HandleNavigation(button);
                    SetDefaultStyle(button);
                }
            }

            // Optional: Set initial selection
            if (_navButtons.TryGetValue("Home", out var homeBtn))
                HandleNavigation(homeBtn);
        }

        private void HandleNavigation(Button selectedButton)
        {
            foreach (var btn in _navButtons.Values)
                SetDefaultStyle(btn);

            SetSelectedStyle(selectedButton);

            // Your navigation logic can go here, e.g.:
            string destination = selectedButton.Content?.ToString();
            NavigateTo(destination);
        }

        private void SetDefaultStyle(Button button)
        {
            button.Background = _defaultColor;
            button.Foreground = _white;
            button.BorderBrush = Brushes.Transparent;
            button.CornerRadius = new CornerRadius(8);
            button.Padding = new Thickness(8, 4);
        }

        private void SetSelectedStyle(Button button)
        {
            button.Background = _selectedColor;
        }

        private void NavigateTo(string? page)
        {
            switch (page)
            {
                case "Home":
                    // Navigator.Instance.Navigate(new HomePage());
                    break;
                case "My Requests":
                    // Navigator.Instance.Navigate(new RequestsPage());
                    break;
                case "Task Execution":
                    // Navigator.Instance.Navigate(new ExecutionPage());
                    break;
                case "Questions":
                    // ...
                    break;
                case "Profile":
                    // ...
                    break;
                case "Log out":
                    // Perform logout
                    break;
            }
        }
    }
}
