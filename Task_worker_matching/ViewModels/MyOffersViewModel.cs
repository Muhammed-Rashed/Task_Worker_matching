using System.Collections.ObjectModel;
using System.Windows.Input;
using Task_worker_matching.Memory_Layer;
using Task_worker_matching.ServiceLayer;

namespace Task_worker_matching.ViewModels
{
    public class MyOffersViewModel : ViewModelBase
    {
        private readonly WorkerOfferService _service = new();
        private readonly NavigationService _nav = NavigationService.Instance;

        public ObservableCollection<Offer> Offers { get; set; }

        public ICommand ViewRequestCommand { get; }
        public ICommand CancelOfferCommand { get; }

        // Navigation commands
        public ICommand NavigateHomeCommand         { get; }
        public ICommand NavigateMyOffersCommand     { get; }
        public ICommand NavigateOpenRequestsCommand { get; }
        public ICommand NavigateExecutionCommand    { get; }
        public ICommand NavigateQuestionsCommand    { get; }
        public ICommand NavigateProfileCommand      { get; }
        public ICommand LogoutCommand               { get; }

        public MyOffersViewModel()
        {
            Offers = new ObservableCollection<Offer>();
            LoadOffers();

            // Commands for offer actions
            ViewRequestCommand = new RelayCommand<Offer>(ViewRequest);
            CancelOfferCommand = new RelayCommand<Offer>(CancelOffer);

            // Navigation commands
            NavigateHomeCommand = new RelayCommand(() => _nav.NavigateTo("WorkerHome"));
            NavigateMyOffersCommand = new RelayCommand(() => _nav.NavigateTo("MyOffersPage"));
            NavigateOpenRequestsCommand = new RelayCommand(() => _nav.NavigateTo("OpenRequestsPage"));
            NavigateExecutionCommand = new RelayCommand(() => _nav.NavigateTo("ExecutionPage"));
            NavigateQuestionsCommand = new RelayCommand(() => _nav.NavigateTo("QuestionsPage"));
            NavigateProfileCommand = new RelayCommand(() => _nav.NavigateTo("WorkerProfile"));
            LogoutCommand = new RelayCommand(() => _nav.Logout());
        }

        private void LoadOffers()
        {
            Offers.Clear();
            foreach (var offer in _service.get_data())
            {
                Offers.Add(offer);
            }
        }

        private void ViewRequest(Offer offer)
        {
            
        }


        private void CancelOffer(Offer offer)
        {
            if (_service.delete(offer))
            {
                Offers.Remove(offer);
            }
        }
    }
}
