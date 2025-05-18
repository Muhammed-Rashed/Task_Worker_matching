using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Task_worker_matching.Memory_Layer;
using Task_worker_matching.ServiceLayer;
using System.Linq;
using Task_worker_matching.Controllers;
using Task_worker_matching.Views;

namespace Task_worker_matching.ViewModels
{
    public class OpenRequestsViewModel : INotifyPropertyChanged
    {
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

            NavigateHomeCommand = new RelayCommand(() => Navigator.Instance.Navigate(new WorkerHome()));
            NavigateMyOffersCommand = new RelayCommand(() => Navigator.Instance.Navigate(new MyOffers()));
            NavigateExecutionCommand = new RelayCommand(() => Navigator.Instance.Navigate(new TaskExecution()));
            NavigateQuestionsCommand = new RelayCommand(() => Navigator.Instance.Navigate(new QuestionsPage()));
            NavigateProfileCommand = new RelayCommand(() => Navigator.Instance.Navigate(new WorkerProfile()));
            LogoutCommand = new RelayCommand(() => Navigator.Instance.Navigate(new Login()));
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
