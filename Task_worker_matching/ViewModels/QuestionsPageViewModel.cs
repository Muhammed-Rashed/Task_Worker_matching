using System.Collections.ObjectModel;

namespace MyAvaloniaApp.ViewModels;

public class QuestionsPageViewModel
{
    public ObservableCollection<QuestionViewModel> Questions { get; set; }

    public QuestionsPageViewModel()
    {
        // Dummy data
        Questions = new ObservableCollection<QuestionViewModel>
        {
            new QuestionViewModel
            {
                Title = "How do I optimize my website for mobile devices?",
                Summary = "I'm having issues with my website's mobile responsiveness..."
            },
            new QuestionViewModel
            {
                Title = "What's the best way to handle user authentication?",
                Summary = "Looking for secure authentication methods for my web app..."
            }
        };
    }
}