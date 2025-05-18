using System.Collections.Generic;
using PersistenceLayer;
using Task_worker_matching.Memory_Layer;

namespace Task_worker_matching.ServiceLayer;

public class QuestionService : IDataService<Question>
{
    private Cache<Question> cache = new Cache<Question>(new QuestionsList(), new QuestionRepositoryStrategy());
    public bool add(Question item)
    {
        return cache.add(item);
    }

    public bool delete(Question item)
    {
        return cache.add(item);
    }

    public List<Question> get_data()
    {
        return cache.get_data();
    }

    public bool update(Question new_item, Question old_item)
    {
        return update(new_item, old_item);
    }

    public bool add_answer(Answer ans, int question_id)
    {
        return cache.add_answer(ans, question_id);
    }
}
