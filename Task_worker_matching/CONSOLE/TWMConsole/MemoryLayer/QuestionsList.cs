using System;
using System.Collections.Generic;

namespace Task_worker_matching.Memory_Layer;

public class QuestionsList : IMemory<Question>
{
    private List<Question> questions = new List<Question>();

    public bool AddItem(Question item)
    {
        if (item != null)
        {
            questions.Add(item);
            return true;
        }
        return false;
    }

    public bool Update(Question newItem, Question oldItem)
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

    public bool DeleteItem(Question item)
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

    public void Set_Data(List<Question> newQuestions)
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

    public List<Question> Get_Data()
    {
        return questions;
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

    public bool add_answer(Answer ans, int id)
    {
        foreach (var question in questions)
        {
            if (question.get_id() == id)
            {
                question.add_answer(ans);
                return true;
            }
        }
        return false;
    }

}
