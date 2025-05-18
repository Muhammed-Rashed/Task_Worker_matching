// File: ViewModels/WorkerProfileViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Task_worker_matching.Controllers;
using Task_worker_matching.Memory_Layer;
using Task_worker_matching.ServiceLayer;
using Task_worker_matching.Views;

namespace Task_worker_matching.ViewModels
{
    public class WorkerProfileViewModel : INotifyPropertyChanged
    {
        // Profile fields
        private string _fullName = "";
        public  string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        private string _email = "";
        public  string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string _phoneNumber = "";
        public  string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(); }
        }

        // Password change fields
        public string CurrentPassword { get; set; } = "";
        public string NewPassword     { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";
        private string _passwordError = "";
        public  string PasswordError
        {
            get => _passwordError;
            private set { _passwordError = value; OnPropertyChanged(); }
        }

        // Collections displayed in the view
        public ObservableCollection<string> Specialities      { get; }
        public ObservableCollection<string> ServiceLocations  { get; }
        public ObservableCollection<string> AvailabilitySlots { get; }
        public ObservableCollection<EarningViewModel> Earnings { get; }

        // Commands
        public ICommand SaveProfileCommand       { get; }
        public ICommand SavePasswordCommand      { get; }
        public ICommand EditSpecialitiesCommand  { get; }
        public ICommand EditLocationsCommand     { get; }
        public ICommand EditAvailabilityCommand  { get; }
        public ICommand NavigateHomeCommand      { get; }
        public ICommand NavigateMyOffersCommand  { get; }
        public ICommand NavigateOpenRequestsCommand { get; }
        public ICommand NavigateExecutionCommand { get; }
        public ICommand NavigateQuestionsCommand { get; }
        public ICommand NavigateProfileCommand   { get; }
        public ICommand LogoutCommand            { get; }

        private readonly AccountService _accountService;
        private readonly RequestService _requestService;

        public WorkerProfileViewModel()
        {
            // Instantiate services
            _accountService = new AccountService();
            _requestService = new RequestService();

            // Grab the current user and cast to Worker
            var user = _accountService.GetCurrentUser();
            var worker = user as Worker
                ?? throw new InvalidOperationException("Current user is not a Worker");

            // Initialize profile fields
            FullName    = worker.get_name();
            Email       = worker.get_email();
            PhoneNumber = worker.get_phone_number();

            // Populate collections directly from the Worker
            Specialities      = new ObservableCollection<string>(worker.GetSpecialities());
            ServiceLocations  = new ObservableCollection<string>(
                worker.GetAvailableLocations().Split(',', StringSplitOptions.RemoveEmptyEntries)
            );
            AvailabilitySlots = new ObservableCollection<string>
            {
                $"{worker.GetAvailableStartTime():hh\\:mm} - {worker.GetAvailableEndTime():hh\\:mm}"
            };
            

            // Commands
            SaveProfileCommand      = new RelayCommand(SaveProfile);
            SavePasswordCommand     = new RelayCommand(ChangePassword);
            EditSpecialitiesCommand = new RelayCommand(() => {/* open edit dialog */});
            EditLocationsCommand    = new RelayCommand(() => {/* open edit dialog */});
            EditAvailabilityCommand = new RelayCommand(() => {/* open edit dialog */});

            NavigateHomeCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new WorkerHome()));
            NavigateMyOffersCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new MyOffers()));
            NavigateOpenRequestsCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new OpenRequests()));
            NavigateExecutionCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new TaskExecution()));
            NavigateQuestionsCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new QuestionsPage()));
            NavigateProfileCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new WorkerProfile()));
            LogoutCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new Login()));
        }

        private void SaveProfile()
        {
            // Create a new Worker with updated values
            var updated = new Worker();
            updated.set_name(FullName);
            updated.set_email(Email);
            updated.set_phone_number(PhoneNumber);
            // preserve other fields from the original
            var original = (Worker)_accountService.GetCurrentUser();
            updated.SetId(original.GetId());
            updated.SetAvailableLocations(original.GetAvailableLocations());
            updated.SetSpecialities(original.GetSpecialities());
            updated.SetAvailableStartTime(original.GetAvailableStartTime());
            updated.SetAvailableEndTime(original.GetAvailableEndTime());
            updated.SetRequestsExecuted(original.GetRequestsExecuted());

            // Push update through AccountService
            bool success = _accountService.update_user(updated);
            // Optionally display a message on failure/success
        }

        private void ChangePassword()
        {
            if (NewPassword != ConfirmPassword)
            {
                PasswordError = "Passwords do not match";
                return;
            }

            // Since AccountService has no change‑password method,
            // we’ll fake it by updating the user’s password and saving:
            var worker = (Worker)_accountService.GetCurrentUser();
            worker.set_password(NewPassword);

            bool ok = _accountService.update_user(worker);
            PasswordError = ok ? "" : "Unable to change password";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
