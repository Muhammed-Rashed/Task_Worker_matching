using Avalonia.Controls;
using Avalonia.Interactivity;
using Task_worker_matching.Controllers;

namespace Task_worker_matching.Views
{
    public partial class WorkerHome : UserControl
    {
        private readonly NavbarController _navbarController = new();

        public WorkerHome()
        {
            InitializeComponent();
            _navbarController.Init(this.FindControl<StackPanel>("NavPanel"));
        }
    }
}
