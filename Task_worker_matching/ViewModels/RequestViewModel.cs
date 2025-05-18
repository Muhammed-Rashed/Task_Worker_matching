using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Task_worker_matching.Memory_Layer;
using Task_worker_matching.ServiceLayer;

namespace Task_worker_matching.ViewModels
{
    public class RequestViewModel : INotifyPropertyChanged
    {
        // Displayed in the card
        public string Title   { get; }
        public string Summary { get; }

        public ICommand ViewDetailsCommand { get; }

        public RequestViewModel(Request memoryModel)
        {
            // Use the Task’s Name as the title
            Title   = memoryModel.Task?.Name ?? "—";
            // Use Request.Description as the summary
            Summary = memoryModel.Description;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
