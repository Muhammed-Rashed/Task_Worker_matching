using System.Collections.Generic;
using Task_worker_matching.Memory_Layer;

namespace ServiceLayer;

public class QuestionService : IDataService<Question>
{
    private Cache<Question> cache;
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
}
