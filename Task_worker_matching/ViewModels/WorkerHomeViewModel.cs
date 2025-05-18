using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Task_worker_matching.Memory_Layer;
using Task_worker_matching.ServiceLayer;

namespace Task_worker_matching.ViewModels
{
    public class WorkerHomeViewModel : INotifyPropertyChanged
    {
        // --- Stats ---
        public int OpenOffersCount     { get; }
        public int AssignedTasksCount  { get; }
        public double Rating           { get; }
        public double CompletionRate   { get; }

        // --- Private Requests ---
        private string _requestSearchQuery = string.Empty;
        public string RequestSearchQuery
        {
            get => _requestSearchQuery;
            set { _requestSearchQuery = value; OnPropertyChanged(); }
        }

        public ObservableCollection<RequestViewModel> PrivateRequests { get; }

        // --- Navigation & Filter Commands ---
        public ICommand NavigateHomeCommand           { get; }
        public ICommand NavigateMyOffersCommand       { get; }
        public ICommand NavigateOpenRequestsCommand   { get; }
        public ICommand NavigateExecutionCommand      { get; }
        public ICommand NavigateQuestionsCommand      { get; }
        public ICommand NavigateProfileCommand        { get; }
        public ICommand LogoutCommand                 { get; }
        public ICommand FilterRequestsCommand         { get; }

        public WorkerHomeViewModel()
        {
            var offerService   = new WorkerOfferService();
            var requestService = new RequestService();
            var taskService    = new TaskService();
            var nav            = NavigationService.Instance;

            // Stats
            OpenOffersCount    = offerService.GetOpenOffersForCurrentWorker().Count;
            AssignedTasksCount = taskService.GetAssignedTasksForCurrentWorker().Count;
            Rating             = requestService.GetOverallRatingForCurrentWorker();
            CompletionRate     = taskService.GetCompletionRateForCurrentWorker();

            OnPropertyChanged(nameof(OpenOffersCount));
            OnPropertyChanged(nameof(AssignedTasksCount));
            OnPropertyChanged(nameof(Rating));
            OnPropertyChanged(nameof(CompletionRate));

            // Private requests
            var requests = requestService.GetPrivateRequestsForCurrentWorker();
            PrivateRequests = new ObservableCollection<RequestViewModel>(
                requests.Select(r => new RequestViewModel(r))
            );

            // Commands
            NavigateHomeCommand         = new RelayCommand(() => nav.NavigateTo("WorkerHome"));
            NavigateMyOffersCommand     = new RelayCommand(() => nav.NavigateTo("MyOffersPage"));
            NavigateOpenRequestsCommand = new RelayCommand(() => nav.NavigateTo("OpenRequestsPage"));
            NavigateExecutionCommand    = new RelayCommand(() => nav.NavigateTo("ExecutionPage"));
            NavigateQuestionsCommand    = new RelayCommand(() => nav.NavigateTo("QuestionsPage"));
            NavigateProfileCommand      = new RelayCommand(() => nav.NavigateTo("ProfilePage"));
            LogoutCommand               = new RelayCommand(() => nav.Logout());

            FilterRequestsCommand = new RelayCommand(ApplyRequestFilter);
        }

        private void ApplyRequestFilter()
{
    var allRequests = new RequestService()
        .GetPrivateRequestsForCurrentWorker();

    var filtered = allRequests
        .Where(r => 
            (r.Task?.Name ?? "")
              .Contains(RequestSearchQuery, StringComparison.OrdinalIgnoreCase)
            || r.Description
              .Contains(RequestSearchQuery, StringComparison.OrdinalIgnoreCase)
        )
        .Select(r => new RequestViewModel(r))
        .ToList();

    PrivateRequests.Clear();
    foreach (var vm in filtered)
        PrivateRequests.Add(vm);
}


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
