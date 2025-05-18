using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Task_worker_matching.Memory_Layer;
using ServiceLayer;
using Task_worker_matching.ServiceLayer;

namespace Task_worker_matching.ViewModels
{
    public class ClientSignUpViewModel : INotifyPropertyChanged
    {
        private readonly AccountService _accountService;
        private string _name;
        private string _phoneNumber;
        private string _email;
        private string _password;
        private string _address;
        private string _paymentInfo;
        private string _errorMessage;
        private bool _hasError;

        public ClientSignUpViewModel()
        {
            _accountService = new AccountService();
            SignUpCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteSignUp, () => true);
            NavigateToLoginCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteNavigateToLogin, () => true);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public string PaymentInfo
        {
            get => _paymentInfo;
            set => SetProperty(ref _paymentInfo, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set 
            { 
                SetProperty(ref _errorMessage, value);
                HasError = !string.IsNullOrEmpty(value);
            }
        }

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public ICommand SignUpCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public event EventHandler<EventArgs> SignUpSuccessful;
        public event EventHandler<EventArgs> NavigateToLogin;

        private void ExecuteSignUp()
        {
            if (ValidateInput())
            {
                var client = new Client();
                client.set_name(Name);
                client.set_email(Email);
                client.set_password(Password);
                client.set_phone_number(PhoneNumber);
                client.set_address(Address);
                client.set_payment_info(PaymentInfo);

                AccountValidation result = _accountService.signup(client);

                switch (result)
                {
                    case AccountValidation.AllCorrect:
                        ErrorMessage = string.Empty;
                        SignUpSuccessful?.Invoke(this, EventArgs.Empty);
                        break;
                    case AccountValidation.EmailWrong:
                        ErrorMessage = "Email already exists or is invalid.";
                        break;
                    case AccountValidation.PasswordWrong:
                        ErrorMessage = "Password is invalid.";
                        break;
                    case AccountValidation.UserWrong:
                        ErrorMessage = "Invalid user type.";
                        break;
                    default:
                        ErrorMessage = "An unknown error occurred.";
                        break;
                }
            }
        }

        private void ExecuteNavigateToLogin()
        {
            NavigateToLogin?.Invoke(this, EventArgs.Empty);
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ErrorMessage = "Name is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                ErrorMessage = "Phone number is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Email address is required.";
                return false;
            }

            if (!Email.Contains("@") || !Email.Contains("."))
            {
                ErrorMessage = "Please enter a valid email address.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Password is required.";
                return false;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "Password must be at least 6 characters.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Address))
            {
                ErrorMessage = "Address is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(PaymentInfo))
            {
                ErrorMessage = "Payment information is required.";
                return false;
            }

            ErrorMessage = string.Empty;
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}