// File: Views/OpenRequests.axaml.cs
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Task_worker_matching.Views
{
    public partial class OpenRequests : UserControl
    {
        public OpenRequests()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
