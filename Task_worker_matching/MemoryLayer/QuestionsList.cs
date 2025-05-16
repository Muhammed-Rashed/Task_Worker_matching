using System;
using System.Collections.Generic;

namespace MyAvaloniaApp.Memory_Layer;

public class QuestionsList
{
    private List<Question> questions = new List<Question>();

    public bool add(Question item)
    {
        if (item != null)
        {
            questions.Add(item);
            return true;
        }
        return false;
    }

    public bool update(Question newItem, Question oldItem)
    {
        if (newItem == null || oldItem == null)
            return false;

        int index = questions.IndexOf(oldItem);
        if (index != -1)
        {
            questions[index] = newItem;
            return true;
        }
        return false;
    }

    public bool delete(Question item)
    {
        if (item != null)
        {
            return questions.Remove(item);
        }
        return false;
    }

    public List<Question> get_data()
    {
        return questions;
    }

    public void set_data(List<Question> newQuestions)
    {
        if (newQuestions != null)
        {
            questions = newQuestions;
        }
    }

    public bool IsEmpty()
    {
        return questions.Count == 0;
    }

    public Question get_byID(int id)
    {
        foreach (var question in questions)
        {
            if (question.get_id() == id)
                return question;
        }
        return null;
    }
}
