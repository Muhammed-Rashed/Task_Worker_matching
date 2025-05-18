using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Task_worker_matching.Memory_Layer;   // note underscore

namespace Task_worker_matching.ViewModels
{
    public class WorkerViewModel : INotifyPropertyChanged
    {
        public string Name             { get; }
        public double Rating           { get; }
        public int ReviewCount         { get; }
        public ObservableCollection<string> Skills { get; }
        public string LocationTag      { get; }
        public string Availability     { get; }
        public string AvailabilityColor { get; }

        public ICommand MoreInfoCommand    { get; }
        public ICommand SendRequestCommand { get; }

        public WorkerViewModel(Worker memoryModel)
        {
            // Map User fields
            Name        = memoryModel.get_name();
            Rating      = memoryModel.get_overall_rating();
            ReviewCount = memoryModel.GetRequestsExecuted().Count;

            // Map Worker-specific fields
            Skills        = new ObservableCollection<string>(memoryModel.GetSpecialities());
            LocationTag   = memoryModel.GetAvailableLocations();

            // Simple availability check based on current time
            var now = DateTime.Now.TimeOfDay;
            var start = memoryModel.GetAvailableStartTime();
            var end   = memoryModel.GetAvailableEndTime();
            bool isAvailable = start <= now && now <= end;

            Availability      = isAvailable ? "Available" : "Unavailable";
            AvailabilityColor = isAvailable ? "LightGreen" : "Red";

            // Commands (dummy implementations)
            MoreInfoCommand    = new RelayCommand(() => {
                // e.g. raise an event or popup with details
            });
            SendRequestCommand = new RelayCommand(() => {
                // e.g. call RequestService.Instance.CreateRequest(...)
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
