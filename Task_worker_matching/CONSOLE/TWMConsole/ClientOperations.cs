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

        public void ShowClientMenu()
        {
            while (true)
            {
                Console.WriteLine("\nClient Menu:");
                Console.WriteLine("1. Home");
                Console.WriteLine("2. My Requests");
                Console.WriteLine("3. Task Execution");
                Console.WriteLine("4. Received Offers");
                Console.WriteLine("5. Make a new Request");
                Console.WriteLine("6. Make a new Task");
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
                    case "5":
                        AddRequest();
                        break;
                    case "6":
                        AddTask();
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

        private void AddRequest()
        {
            // 1) Choose Task
            Console.Out.WriteLine("Select a Task:");
            var tasks = _taskService.get_data();
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.Out.WriteLine($"{i + 1}) {tasks[i].Name}");
            }

            int taskChoice;
            while (true)
            {
                Console.Out.Write("Enter number: ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out taskChoice)
                    && taskChoice >= 1
                    && taskChoice <= tasks.Count)
                {
                    break;
                }
                Console.Error.WriteLine("Invalid choice. Please enter a valid task number.");
            }

            var request = new Request
            {
                Task = tasks[taskChoice - 1]
            };

            // 2) preferred_date
            DateTime preferredDate;
            while (true)
            {
                Console.Out.Write("Enter preferred date (yyyy-MM-dd): ");
                if (DateTime.TryParse(Console.ReadLine(), out preferredDate))
                    break;
                Console.Error.WriteLine("Invalid date format. Try again.");
            }
            request.PreferredDate = preferredDate;

            // 3) Address
            Console.Out.Write("Enter address: ");
            request.Address = Console.ReadLine() ?? "";

            // 4) Location
            Console.Out.Write("Enter location: ");
            request.Location = Console.ReadLine() ?? "";

            // 5) Placement_time
            DateTime placementTime;
            while (true)
            {
                Console.Out.Write("Enter placement time (yyyy-MM-dd HH:mm): ");
                if (DateTime.TryParse(Console.ReadLine(), out placementTime))
                    break;
                Console.Error.WriteLine("Invalid date/time format. Try again.");
            }
            request.PlacementTime = placementTime;

            // 6) Description
            Console.Out.Write("Enter description: ");
            request.Description = Console.ReadLine() ?? "";

            // 7) Fee
            decimal fee;
            while (true)
            {
                Console.Out.Write("Enter fee (e.g. 125.50): ");
                if (decimal.TryParse(Console.ReadLine(), out fee))
                    break;
                Console.Error.WriteLine("Invalid amount. Try again.");
            }
            request.Fee = fee;

            // Now hand off to your service or repository,
            // where you'll also set Client_id, Status, IsPrivate, etc.
            bool success = _requestService.add(request);

            Console.Out.WriteLine(success
                ? "Request submitted successfully!"
                : "Failed to submit request.");
        }

        private void AddTask()
        {
            Console.Out.Write("Enter new task name: ");
            string name = Console.ReadLine()?.Trim() ?? "";
            while (string.IsNullOrWhiteSpace(name))
            {
                Console.Error.WriteLine("Task name cannot be empty.");
                Console.Out.Write("Enter new task name: ");
                name = Console.ReadLine()?.Trim() ?? "";
            }

            long avgTime;
            Console.Out.Write("Enter average time (in seconds): ");
            while (!long.TryParse(Console.ReadLine(), out avgTime) || avgTime < 0)
            {
                Console.Error.WriteLine("Please enter a non-negative integer.");
                Console.Out.Write("Enter average time (in seconds): ");
            }

            decimal avgFee;
            Console.Out.Write("Enter average fee (e.g. 150.00): ");
            while (!decimal.TryParse(Console.ReadLine(), out avgFee) || avgFee < 0)
            {
                Console.Error.WriteLine("Please enter a valid non-negative amount.");
                Console.Out.Write("Enter average fee (e.g. 150.00): ");
            }

            Console.Out.WriteLine("Enter specialities (comma-separated). Missing ones will be created:");
            Console.Out.Write("Specialities: ");
            var specNames = Console.ReadLine()?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();

            if (specNames.Count == 0)
            {
                Console.Error.WriteLine("At least one speciality is required.");
                return;
            }

            var task = new Memory_Layer.Task();

            task.Name = name;
            task.AVG_Fee = avgFee;
            task.Avg_Time = avgTime;
            task.Specialties = specNames;

            bool success = _taskService.add(task);

            Console.Out.WriteLine(success
                ? "Task created successfully!"
                : "Failed to create task.");
        }


    }
}
