using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Task_worker_matching.Memory_Layer;
using Task_worker_matching.ServiceLayer;
using System.Linq;
using System.Collections.Generic;

namespace Task_worker_matching.ViewModels
{
    public class OpenRequestsViewModel : INotifyPropertyChanged
    {
        private readonly NavigationService _nav = NavigationService.Instance;
        private readonly RequestService _requestService = new();

        public ObservableCollection<Request> OpenRequests { get; }

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateMyOffersCommand { get; }
        public ICommand NavigateExecutionCommand { get; }
        public ICommand NavigateQuestionsCommand { get; }
        public ICommand NavigateProfileCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand FilterRequestsCommand { get; }

        public OpenRequestsViewModel()
        {
            OpenRequests = new ObservableCollection<Request>(
                _requestService.get_data()
                               .Where(r => !r.IsPrivate)
                               .ToList()
            );

            NavigateHomeCommand = new RelayCommand(() => _nav.NavigateTo("WorkerHome"));
            NavigateMyOffersCommand = new RelayCommand(() => _nav.NavigateTo("MyOffersPage"));
            NavigateExecutionCommand = new RelayCommand(() => _nav.NavigateTo("ExecutionPage"));
            NavigateQuestionsCommand = new RelayCommand(() => _nav.NavigateTo("QuestionsPage"));
            NavigateProfileCommand = new RelayCommand(() => _nav.NavigateTo("WorkerProfile"));
            LogoutCommand = new RelayCommand(() => _nav.Logout());
            FilterRequestsCommand = new RelayCommand(FilterRequests);
        }

        private void FilterRequests()
        {
            // Basic filtering logic. Extend this as needed.
            var filtered = _requestService.get_data()
                .Where(r => !r.IsPrivate)
                .ToList();

            OpenRequests.Clear();
            foreach (var request in filtered)
                OpenRequests.Add(request);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
