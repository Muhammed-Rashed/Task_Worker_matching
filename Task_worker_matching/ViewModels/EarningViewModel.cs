using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Task_worker_matching.Memory_Layer;

namespace Task_worker_matching.ViewModels
{
    public class EarningViewModel : INotifyPropertyChanged
    {
        public string TaskTitle       { get; }
        public DateTime DateCompleted { get; }
        public decimal Amount         { get; }
        public string ClientName      { get; }

        public EarningViewModel(Request memoryModel)
        {
            // Map fields
            TaskTitle     = memoryModel.Task?.Name ?? "<unknown>";
            DateCompleted = memoryModel.PreferredDate;  // or PlacementTime
            Amount        = memoryModel.Fee;

            // Stubbed client name until you add a proper lookup:
            ClientName    = $"Client #{memoryModel.ClientId}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
