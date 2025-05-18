using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Task_worker_matching.Controllers;
using Task_worker_matching.Views;
using Task_worker_matching.ServiceLayer;

namespace Task_worker_matching.ViewModels
{
    public class WorkerHomeViewModel : INotifyPropertyChanged
    {
        public int OpenOffersCount { get; }
        public int AssignedTasksCount { get; }
        public double Rating { get; }
        public double CompletionRate { get; }

        private string _requestSearchQuery = string.Empty;
        public string RequestSearchQuery
        {
            get => _requestSearchQuery;
            set { _requestSearchQuery = value; OnPropertyChanged(); }
        }

        public ObservableCollection<RequestViewModel> PrivateRequests { get; }

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateMyOffersCommand { get; }
        public ICommand NavigateOpenRequestsCommand { get; }
        public ICommand NavigateExecutionCommand { get; }
        public ICommand NavigateQuestionsCommand { get; }
        public ICommand NavigateProfileCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand FilterRequestsCommand { get; }

        public WorkerHomeViewModel()
        {
            var offerService = new WorkerOfferService();
            var requestService = new RequestService();
            var taskService = new TaskService();

            OpenOffersCount = offerService.GetOpenOffersForCurrentWorker().Count;
            AssignedTasksCount = taskService.GetAssignedTasksForCurrentWorker().Count;
            Rating = requestService.GetOverallRatingForCurrentWorker();
            CompletionRate = taskService.GetCompletionRateForCurrentWorker();

            OnPropertyChanged(nameof(OpenOffersCount));
            OnPropertyChanged(nameof(AssignedTasksCount));
            OnPropertyChanged(nameof(Rating));
            OnPropertyChanged(nameof(CompletionRate));

            var requests = requestService.GetPrivateRequestsForCurrentWorker();
            PrivateRequests = new ObservableCollection<RequestViewModel>(
                requests.Select(r => new RequestViewModel(r))
            );

            NavigateHomeCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new WorkerHome()));
            NavigateMyOffersCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new MyOffers()));
            NavigateOpenRequestsCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new OpenRequests()));
            NavigateExecutionCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new TaskExecution()));
            NavigateQuestionsCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new QuestionsPage()));
            NavigateProfileCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new WorkerProfile()));
            LogoutCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new Login()));

            FilterRequestsCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ApplyRequestFilter);
        }

        private void ApplyRequestFilter()
        {
            var allRequests = new RequestService().GetPrivateRequestsForCurrentWorker();
            var filtered = allRequests
                .Where(r =>
                    (r.Task?.Name ?? "").Contains(RequestSearchQuery, StringComparison.OrdinalIgnoreCase)
                    || r.Description.Contains(RequestSearchQuery, StringComparison.OrdinalIgnoreCase)
                )
                .Select(r => new RequestViewModel(r))
                .ToList();

            PrivateRequests.Clear();
            foreach (var vm in filtered)
                PrivateRequests.Add(vm);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
