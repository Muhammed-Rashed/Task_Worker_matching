using System;

namespace Task_worker_matching.Memory_Layer;

public class Answer
{
    private User answerer;           // Reference to User who answered
    private string answer;
    private DateTime answer_time;
    private double answer_rate;

    public Answer(User answerer, string answer, DateTime time)
    {
        this.answerer = answerer;
        this.answer = answer;
        this.answer_time = time;
        this.answer_rate = 0.0;
    }

    public User get_answerer() => answerer;
    public string get_answer() => answer;
    public DateTime get_answer_time() => answer_time;
    public double get_answer_rate() => answer_rate;

    public void set_answerer(User answerer) => this.answerer = answerer;
    public void set_answer(string answer) => this.answer = answer;
    public void set_answer_time(DateTime time) => this.answer_time = time;
    public void set_answer_rate(double rate) => this.answer_rate = rate;
}
