// File: ViewModels/WorkerSignUpViewModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Task_worker_matching.Controllers;
using Task_worker_matching.Memory_Layer;
using Task_worker_matching.ServiceLayer;
using Task_worker_matching.Views;

namespace Task_worker_matching.ViewModels
{
    public class WorkerSignUpViewModel : INotifyPropertyChanged
    {
        private readonly AccountService _accountService = new();


        // Properties bound to the UI
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }
        private string _fullName = "";

        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(); }
        }
        private string _phoneNumber = "";

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }
        private string _email = "";

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }
        private string _password = "";

        public string Specialities
        {
            get => _specialities;
            set { _specialities = value; OnPropertyChanged(); }
        }
        private string _specialities = "";

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }
        private string _errorMessage = "";

        // Commands
        public ICommand RegisterCommand { get; }
        public ICommand NavigateLoginCommand { get; }

        public WorkerSignUpViewModel()
        {
            RegisterCommand = new RelayCommand(RegisterWorker);
            NavigateLoginCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new Login()));
        }

        private void RegisterWorker()
        {
            var worker = new Worker();

            // Set base User properties using setters
            worker.set_name(FullName);
            worker.set_phone_number(PhoneNumber);
            worker.set_email(Email);
            worker.set_password(Password);

            // Parse specialities into List<string> and assign
            var specialitiesList = Specialities
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToList();

            worker.SetSpecialities(specialitiesList);

            var result = _accountService.signup(worker);

            if (result == AccountValidation.AllCorrect)
            {
                new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new WorkerHome()));
            }
            else
            {
                ErrorMessage = result.ToString(); // Optionally improve this message
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
