using System.Collections.ObjectModel;

namespace Task_worker_matching.ViewModels
{
    public class ClientTaskExecutionPageViewModel
    {
        public ObservableCollection<TaskDisplayItem> Tasks { get; set; }

        public ClientTaskExecutionPageViewModel()
        {
            Tasks = new ObservableCollection<TaskDisplayItem>
            {
                new TaskDisplayItem
                {
                    Title = "Install Wi-Fi Camera",
                    UserName = "Sarah Connor",
                    UserRole = "Client",
                    Status = "Ongoing",
                    Date = "2025-05-17",
                    Address = "22 Jump Street",
                    Description = "Install outdoor security camera with motion alerts.",
                    Rating = "4.8",
                    ImagePath = "Assets/user1.png"
                },
                new TaskDisplayItem
                {
                    Title = "Fix AC",
                    UserName = "John Wick",
                    UserRole = "Client",
                    Status = "Completed",
                    Date = "2025-05-16",
                    Address = "101 Continental Avenue",
                    Description = "Air conditioner making noise. Needs filter and coolant check.",
                    Rating = "5.0",
                    ImagePath = "Assets/user2.png"
                }
            };
        }
    }

    public class TaskDisplayItem
    {
        public string Title { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Rating { get; set; }
        public string ImagePath { get; set; }
    }
}