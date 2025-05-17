using Avalonia.Controls;
using MyAvaloniaApp.Controllers;

namespace MyAvaloniaApp.Views
{
    public partial class QuestionsPage : UserControl
    {
        private readonly NavbarController _navbarController = new();
        public QuestionsPage()
        {
            InitializeComponent();
            _navbarController.Init(this.FindControl<StackPanel>("NavPanel"));
        }
    }
}
