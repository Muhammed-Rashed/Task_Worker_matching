using Avalonia.Controls;
using Task_worker_matching.Controllers;

namespace Task_worker_matching.Views
{
    public partial class WorkerProfile : UserControl
    {
        private readonly NavbarController _navbarController = new();

        public WorkerProfile()
        {
            InitializeComponent();
            _navbarController.Init(this.FindControl<StackPanel>("NavPanel"));
        }
    }
}
