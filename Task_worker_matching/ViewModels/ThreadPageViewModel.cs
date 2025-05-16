using System.Collections.ObjectModel;

namespace Task_worker_matching.ViewModels;

public class ThreadPageViewModel
{
    public string QuestionTitle { get; set; }
    public string QuestionBody { get; set; }

    public ObservableCollection<AnswerViewModel> Answers { get; set; }

    public ThreadPageViewModel()
    {
        QuestionTitle = "How do I optimize my website for mobile devices?";
        QuestionBody = "I'm having issues with my website's mobile responsiveness...";

        Answers = new ObservableCollection<AnswerViewModel>
        {
            new AnswerViewModel
            {
                Author = "John Doe (Web Dev)",
                Content = "Try mobile-first design with responsive breakpoints..."
            },
            new AnswerViewModel
            {
                Author = "Jane Smith",
                Content = "You should test your site on various screen sizes and devices."
            }
        };
    }
}