using System.Collections.Generic;
using System.Linq;
namespace Task_worker_matching.Memory_Layer;

public class TasksList : IMemory<Task>
{
    private List<Task> _tasks;

    public TasksList()
    {
        _tasks = new List<Task>();
    }

    public bool AddItem(Task item)
    {
        try
        {
            _tasks.Add(item);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool Update(Task new_item, Task old_item)
    {
        try
        {
            var node = _tasks.Find(t => t.Id == old_item.Id);
            if (node != null)
            {
                _tasks.Remove(node);
                _tasks.Add(new_item);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public bool DeleteItem(Task item)
    {
        return _tasks.Remove(item);
    }

    public List<Task> Get_Data()
    {
        return new List<Task>(_tasks);
    }

    public bool IsEmpty()
    {
        return _tasks.Count == 0;
    }

    public List<Task> Set_Data(List<Task> data)
    {
        _tasks = new List<Task>(data);
        return Get_Data();
    }

    public Task Get_ById(int id)
    {
        // Assuming Task has an Id property (though not shown in the diagram)
        // This is an extension to make it work similar to other components
        return _tasks.FirstOrDefault(t => t.GetHashCode() == id);
    }

    void IMemory<Task>.Set_Data(List<Task> data)
    {
        _tasks = data;
    }
}
