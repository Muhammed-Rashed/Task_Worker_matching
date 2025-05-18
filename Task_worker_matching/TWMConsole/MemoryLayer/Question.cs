using System;
using System.Collections.Generic;

namespace Task_worker_matching.Memory_Layer;

public class Question
{
    private User asker;
    private string question;
    private List<Answer> answers;
    private DateTime question_time;
    private double question_rate;
    private int id;

    public int get_id() => id;
    public User get_asker() => asker;
    public string get_question() => question;
    public DateTime get_question_time() => question_time;
    public double get_question_rate() => question_rate;
    public List<Answer> get_answers() => answers;

    public void set_id(int id) => this.id = id;
    public void set_asker(User asker) => this.asker = asker;
    public void set_question(string question) => this.question = question;
    public void set_question_time(DateTime time) => this.question_time = time;
    public void set_question_rate(double rate) => this.question_rate = rate;
    public void set_answers(List<Answer> ans) => this.answers = ans;

    public void add_answer(Answer answer)
    {
        answers.Add(answer);
    }
}
    

    
   
