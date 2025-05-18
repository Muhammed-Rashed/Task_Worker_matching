using System;
using System.Collections.Generic;
using Task_worker_matching.ServiceLayer;
using Task_worker_matching.Memory_Layer;

namespace Task_worker_matching
{
    public class ClientOperations
    {
        private readonly AccountService _accountService;
        private readonly IDataService<Request> _requestService;
        private readonly IDataService<Task_worker_matching.Memory_Layer.Task> _taskService;
        private readonly IDataService<Offer> _offerService;
        private readonly IDataService<RequestExecuted> _requestExecutedService;

        public ClientOperations(
            AccountService accountService,
            IDataService<Request> requestService,
            IDataService<Task_worker_matching.Memory_Layer.Task> taskService,
            IDataService<Offer> offerService,
            IDataService<RequestExecuted> requestExecutedService)
        {
            _accountService = accountService;
            _requestService = requestService;
            _taskService = taskService;
            _offerService = offerService;
            _requestExecutedService = requestExecutedService;
        }

        public void StartLogin()
        {
            Console.Write("Enter email: ");
            string email = Console.ReadLine()?.Trim();
            Console.Write("Enter password: ");
            string password = Console.ReadLine()?.Trim();

            Console.Write("Are you a client or a worker? (client/worker): ");
            string userType = Console.ReadLine()?.Trim().ToLower();

            User user;
            if (userType == "client")
                user = new Client(email, password); // Your Client constructor
            else if (userType == "worker")
                user = new Worker(email, password); // Your Worker constructor
            else
            {
                Console.WriteLine("Invalid user type.");
                return;
            }

            var loginResult = _accountService.login(user, userType);

            if (loginResult == AccountValidation.AllCorrect)
            {
                Console.WriteLine("Login successful!");

                if (userType == "client")
                {
                    ShowClientMenu();
                }
                else
                {
                    Console.WriteLine("Worker menu is not implemented yet.");
                    // TODO: ShowWorkerMenu();
                }
            }
            else
            {
                Console.WriteLine($"Login failed: {loginResult}");
            }
        }

        public void ShowClientMenu()
        {
            while (true)
            {
                Console.WriteLine("\nClient Menu:");
                Console.WriteLine("1. Home");
                Console.WriteLine("2. My Requests");
                Console.WriteLine("3. Task Execution");
                Console.WriteLine("4. Received Offers");
                Console.WriteLine("0. Log Out");
                Console.Write("Select an option: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ShowHome();
                        break;
                    case "2":
                        ShowMyRequests();
                        break;
                    case "3":
                        ShowTaskExecution();
                        break;
                    case "4":
                        ShowReceivedOffers();
                        break;
                    case "0":
                        Console.WriteLine("Logging out...");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }

        private void ShowHome()
        {
            Console.WriteLine("Welcome to your Home Dashboard!");
        }

        private void ShowMyRequests()
        {
            Console.WriteLine("Fetching your requests...");
            var currentUser = _accountService.GetCurrentUser();
            var allRequests = _requestService.get_data();

            foreach (var request in allRequests)
            {
                if (request.ClientId == currentUser.get_user_ID())
                {
                    Console.WriteLine($"- {request.Description} at {request.Location}");
                }
            }
        }

        private void ShowTaskExecution()
        {
            var currentUser = _accountService.GetCurrentUser();
            var allExecutions = _requestExecutedService.get_data();

            var completedExecutions = allExecutions.FindAll(exec =>
                exec.GetClient().get_user_ID() == currentUser.get_user_ID() &&
                exec.GetStatus() == RequestStatus.Completed &&
                exec.GetWorkerRate() == 0
            );

            if (completedExecutions.Count == 0)
            {
                Console.WriteLine("You have no completed requests pending rating.");
                return;
            }

            Console.WriteLine("\nCompleted Executed Requests (Pending Rating):");
            for (int i = 0; i < completedExecutions.Count; i++)
            {
                var req = completedExecutions[i].GetRequest();
                Console.WriteLine($"{i + 1}. {req.Description} at {req.Location}");
            }

            Console.Write("Select a request to review (number): ");
            if (!int.TryParse(Console.ReadLine(), out int selectedIndex) || selectedIndex < 1 || selectedIndex > completedExecutions.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            var selectedExecution = completedExecutions[selectedIndex - 1];

            Console.Write("Do you want to accept this completed task? (y/n): ");
            var decision = Console.ReadLine()?.Trim().ToLower();

            if (decision == "y")
            {
                Console.Write("Enter a rating for the worker (0-5): ");
                if (double.TryParse(Console.ReadLine(), out double rating) && rating >= 0 && rating <= 5)
                {
                    selectedExecution.SetWorkerRate(rating);
                    _requestExecutedService.update(selectedExecution, selectedExecution);
                    Console.WriteLine("Worker rated successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid rating. Please enter a number between 0 and 5.");
                }
            }
            else
            {
                Console.WriteLine("Task rejected.");
            }
        }

        private void ShowReceivedOffers()
        {
            Console.WriteLine("Received Offers:");
            var currentUser = _accountService.GetCurrentUser();
            var allOffers = _offerService.get_data();

            foreach (var offer in allOffers)
            {
                var request = offer.GetRequest();
                if (request.ClientId == currentUser.get_user_ID())
                {
                    var worker = offer.GetWorker();
                    Console.WriteLine($"- Offer from {worker.get_email()} | Price: {offer.GetFee()} | Message: {offer.GetMessage()}");
                }
            }
        }
    }
}
