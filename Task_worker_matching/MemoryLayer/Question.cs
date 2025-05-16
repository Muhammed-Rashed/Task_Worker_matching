using System;
using System.Collections.Generic;

namespace MyAvaloniaApp.Memory_Layer;

public class Question
{
    private User asker;
    private string question;
    private LinkedList<Answer> answers;
    private DateTime question_time;
    private double question_rate;
    private int id;

    public Question(int id, User asker, string question, DateTime question_time)
    {
        this.id = id;
        this.asker = asker;
        this.question = question;
        this.question_time = question_time;
        this.question_rate = 0.0;
        this.answers = new LinkedList<Answer>();
    }

    public int get_id() => id;
    public User get_asker() => asker;
    public string get_question() => question;
    public DateTime get_question_time() => question_time;
    public double get_question_rate() => question_rate;
    public LinkedList<Answer> get_answers() => answers;

    public void set_id(int id) => this.id = id;
    public void set_asker(User asker) => this.asker = asker;
    public void set_question(string question) => this.question = question;
    public void set_question_time(DateTime time) => this.question_time = time;
    public void set_question_rate(double rate) => this.question_rate = rate;

    public void add_answer(Answer answer)
    {
        answers.AddLast(answer);
    }
}
    

    
   
