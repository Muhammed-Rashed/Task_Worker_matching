// File: Views/WorkerSignUp.axaml.cs
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Task_worker_matching.ViewModels;

namespace Task_worker_matching.Views
{
    public partial class WorkerSignUp : UserControl
    {
        public WorkerSignUp()
        {
            InitializeComponent();
            DataContext = new WorkerSignUpViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
