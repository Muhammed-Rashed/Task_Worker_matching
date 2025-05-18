using System.Collections.Generic;
using Task_worker_matching.Memory_Layer;

namespace ServiceLayer;

public class RequestExecutedService : IDataService<RequestExecuted>
{
    private Cache<RequestExecuted> cache;
    public bool add(RequestExecuted item)
    {
        return cache.add(item);
    }

    public bool delete(RequestExecuted item)
    {
        return cache.add(item);
    }

    public List<RequestExecuted> get_data()
    {
        return cache.get_data();
    }

    public bool update(RequestExecuted new_item, RequestExecuted old_item)
    {
        return cache.update(new_item, old_item);
    }
}
