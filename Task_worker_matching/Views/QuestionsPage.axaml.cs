using Avalonia.Controls;
using Task_worker_matching.Controllers;

namespace Task_worker_matching.Views
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
