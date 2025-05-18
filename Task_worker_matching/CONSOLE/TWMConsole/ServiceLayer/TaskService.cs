using System;
using System.Collections.Generic;
using Task_worker_matching.Memory_Layer;
using DomainTask = Task_worker_matching.Memory_Layer.Task;

namespace Task_worker_matching.ServiceLayer;

class TaskService : IDataService<DomainTask>
{
    private Cache<DomainTask> cache = new(new TasksList(), new TaskRepoStrategy());
    
    public bool isValidItem(DomainTask item)
    {
        return item.Avg_Time > 0 && item.AVG_Fee > 0 && item.Specialties != null;
    }
    
    public bool add(DomainTask item)
    {
        if (isValidItem(item))
            return cache.add(item);
        
        return false;
    }
    
    public bool update(DomainTask new_item, DomainTask old_item)
    {
        if(isValidItem(new_item))
            return cache.update(new_item, old_item);
        
        return false;
    }
    
    public bool delete(DomainTask item)
    {
        if(isValidItem(item))
            return cache.delete(item);
        
        return false;
    }
    
    public List<DomainTask> get_data()
    {
        return cache.get_data();
    }
}
