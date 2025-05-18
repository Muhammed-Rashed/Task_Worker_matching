using System.Collections.ObjectModel;
using System.Windows.Input;
using Task_worker_matching.Controllers;
using Task_worker_matching.Memory_Layer;
using Task_worker_matching.ServiceLayer;
using Task_worker_matching.Views;

namespace Task_worker_matching.ViewModels
{
    public class MyOffersViewModel : ViewModelBase
    {
        private readonly WorkerOfferService _service = new();

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
            NavigateHomeCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new WorkerHome()));
            NavigateMyOffersCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new MyOffersPage()));
            NavigateOpenRequestsCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new OpenRequests()));
            NavigateExecutionCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new TaskExecution()));
            NavigateQuestionsCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new QuestionsPage()));
            NavigateProfileCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new WorkerProfile()));
            LogoutCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => Navigator.Instance.Navigate(new Login()));
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
