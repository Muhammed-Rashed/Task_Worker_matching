using System;
using System.Collections.Generic;
using Task_worker_matching.Memory_Layer;

namespace Task_worker_matching.ServiceLayer;

class TaskService : IDataService<Task>
{
    private Cache<Task> cache = new(new TasksList(), new TaskRepoStrategy());
    
    public bool isValidItem(Task item)
    {
        return item.Avg_Time > 0 && item.AVG_Fee > 0 && item.Specialties != null;
    }
    
    public bool add(Task item)
    {
        if (isValidItem(item))
            return cache.add(item);
        
        return false;
    }
    
    public bool update(Task new_item, Task old_item)
    {
        if(isValidItem(new_item))
            return cache.update(new_item, old_item);
        
        return false;
    }
    
    public bool delete(Task item)
    {
        if(isValidItem(item))
            return cache.delete(item);
        
        return false;
    }
    
    public List<Task> get_data()
    {
        return cache.get_data();
    }
}
