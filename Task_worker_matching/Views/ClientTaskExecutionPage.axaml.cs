using Avalonia.Controls;
using Task_worker_matching.ViewModels;

namespace Task_worker_matching.Views
{
    public partial class ClientTaskExecutionPage : UserControl
    {
        public ClientTaskExecutionPage()
        {
            InitializeComponent();
            DataContext = new ClientTaskExecutionPageViewModel(); // Set the ViewModel
        }
    }
}