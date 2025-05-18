using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Task_worker_matching.Memory_Layer;
using Task_worker_matching.ServiceLayer;
using PersistenceLayer;
using DomainTask = Task_worker_matching.Memory_Layer.Task;

namespace Task_worker_matching
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup Dependency Injection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Entry point: run the application
            var app = serviceProvider.GetService<App>();
            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // PersistenceManager
            services.AddSingleton(PersistenceManager.GetInstance());

            // Repositories
            services.AddTransient<ClientUserRepository>();
            services.AddTransient<WorkerUserRepository>();
            services.AddTransient<IUserRepository>(sp => sp.GetService<ClientUserRepository>());
            services.AddTransient<IUserRepository>(sp => sp.GetService<WorkerUserRepository>());
            services.AddTransient<RequestRepoStrategy>();
            services.AddTransient<TaskRepoStrategy>();
            services.AddTransient<RequestExecutedRepositoryStrategy>();
            services.AddTransient<WorkerOfferRepoStrategy>();
            services.AddTransient<IRepositoryStrategy<Request>>(sp => sp.GetService<RequestRepoStrategy>());
            services.AddTransient<IRepositoryStrategy<DomainTask>>(sp => sp.GetService<TaskRepoStrategy>());
            services.AddTransient<IRepositoryStrategy<RequestExecuted>>(sp => sp.GetService<RequestExecutedRepositoryStrategy>());
            services.AddTransient<IRepositoryStrategy<Offer>>(sp => sp.GetService<WorkerOfferRepoStrategy>());

            // Memory Layer
            services.AddSingleton<IMemory<Request>, RequestList>();
            services.AddSingleton<IMemory<DomainTask>, TasksList>();
            services.AddSingleton<IMemory<RequestExecuted>, RequestExecutedList>();
            services.AddSingleton<IMemory<Offer>, WorkerOffersList>();

            // Services
            services.AddTransient<AccountService>();
            services.AddTransient<Authenticator>();
            services.AddTransient<RequestService>();
            services.AddTransient<TaskService>();
            services.AddTransient<RequestExecutedService>();
            services.AddTransient<WorkerOfferService>();
            services.AddTransient<IDataService<Request>>(sp => sp.GetService<RequestService>());
            services.AddTransient<IDataService<DomainTask>>(sp => sp.GetService<TaskService>());
            services.AddTransient<IDataService<RequestExecuted>>(sp => sp.GetService<RequestExecutedService>());
            services.AddTransient<IDataService<Offer>>(sp => sp.GetService<WorkerOfferService>());

            // Application entry
            services.AddTransient<App>();
        }
    }

    public class App
    {
        private readonly AccountService _accountService;
        private readonly IDataService<Request> _requestService;
        private readonly IDataService<DomainTask> _taskService;
        private readonly IDataService<Offer> _offerService;

        public App(
            AccountService accountService,
            IDataService<Request> requestService,
            IDataService<DomainTask> taskService,
            IDataService<Offer> offerService)
        {
            _accountService = accountService;
            _requestService = requestService;
            _taskService = taskService;
            _offerService = offerService;
        }

        public void Run()
        {
            Console.WriteLine("Welcome to Task-Worker Matching App");
            while (true)
            {
                Console.WriteLine("\nMain Menu:");
                Console.WriteLine("1. Sign Up");
                Console.WriteLine("2. Log In");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        HandleSignUp();
                        break;
                    case "2":
                        if (HandleLogIn())
                            ShowUserMenu();
                        break;
                    case "0":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }

        private void HandleSignUp()
        {
            Console.WriteLine("\n--- Sign Up ---");
            Console.Write("Email: ");
            var email = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();
            Console.Write("Are you a Worker? (y/n): ");
            var isWorker = Console.ReadLine()?.ToLower() == "y";

            User newUser = isWorker ? new Worker() : new Client();
            newUser.set_email(email);
            newUser.set_password(password);

            var result = _accountService.signup(newUser);
            Console.WriteLine(result == AccountValidation.AllCorrect ? "Sign up successful!" : "Sign up failed.");
        }

        private bool HandleLogIn()
        {
            Console.WriteLine("\n--- Log In ---");
            Console.Write("Email: ");
            var email = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();

            // Attempt login as Worker first, then Client
            User userCandidate = new Worker { }; 
            userCandidate.set_email(email);
            userCandidate.set_password(password);
            var result = _accountService.login(userCandidate);
            if (result != AccountValidation.AllCorrect)
            {
                userCandidate = new Client { };
                userCandidate.set_email(email);
                userCandidate.set_password(password);
                result = _accountService.login(userCandidate);
            }

            if (result == AccountValidation.AllCorrect)
            {
                Console.WriteLine("Login successful!");
                return true;
            }
            Console.WriteLine("Login failed: " + result);
            return false;
        }

        private void ShowUserMenu()
        {
            var currentUser = _accountService.GetCurrentUser();
            bool isWorker = currentUser is Worker;

            while (true)
            {
                Console.WriteLine("\n" + (isWorker ? "Worker Menu:" : "Client Menu:"));
                if (isWorker)
                {
                    Console.WriteLine("1. Home");
                    Console.WriteLine("2. General Requests");
                    Console.WriteLine("3. My Offers");
                    Console.WriteLine("4. Task Execution");
                    Console.WriteLine("0. Log Out");
                }
                else
                {
                    Console.WriteLine("1. Home");
                    Console.WriteLine("2. My Requests");
                    Console.WriteLine("3. Task Execution");
                    Console.WriteLine("4. Received Offers");
                    Console.WriteLine("0. Log Out");
                }

                Console.Write("Select an option: ");
                var choice = Console.ReadLine();

                if (choice == "0")
                {
                    Console.WriteLine("Logging out...");
                    return;
                }

                if (isWorker)
                {
                    switch (choice)
                    {
                        case "1": Console.WriteLine("Home (worker)"); break;
                        case "2": Console.WriteLine("General Requests"); break;
                        case "3": Console.WriteLine("My Offers"); break;
                        case "4": Console.WriteLine("Task Execution (worker)"); break;
                        default: Console.WriteLine("Invalid option. Try again."); break;
                    }
                }
                else
                {
                    switch (choice)
                    {
                        case "1": Console.WriteLine("Home (client)"); break;
                        case "2": Console.WriteLine("My Requests"); break;
                        case "3": Console.WriteLine("Task Execution (client)"); break;
                        case "4": Console.WriteLine("Received Offers"); break;
                        default: Console.WriteLine("Invalid option. Try again."); break;
                    }
                }
            }
        }
    }
}
