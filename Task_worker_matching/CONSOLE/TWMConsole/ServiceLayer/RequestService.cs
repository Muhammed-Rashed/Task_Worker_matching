using System;
using System.Collections.Generic;
using Task_worker_matching.Memory_Layer;

namespace Task_worker_matching.ServiceLayer;
class RequestService : IDataService<Request>
{
    private readonly Cache<Request> cache = new(new RequestList(), new RequestRepoStrategy());
    
    public bool isValidItem(Request item)
    {
        return item.Id > 0 && item.Address != "" && item.Task != null;
    }
    
    public bool add(Request item)
    {
        if (isValidItem(item))
        {
            return cache.add(item);
        }
        return false;
    }
    
    public bool update(Request new_item, Request old_item)
    {
        if(isValidItem(new_item))
        {
            return cache.update(new_item, old_item);
        }
        return false;
    }
    
    public bool delete(Request item)
    {
        if(isValidItem(item))
        {
            return cache.delete(item);
        }
        return false;
    }
    
    public List<Request> get_data()
    {
        return cache.get_data();
    }
    
}
