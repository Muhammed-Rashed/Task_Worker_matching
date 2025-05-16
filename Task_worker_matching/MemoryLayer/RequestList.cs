using System.Collections.Generic;
using System.Linq;

namespace Task_worker_matching.Memory_Layer;

public class RequestList: IMemory<Request>
{
    private LinkedList<Request> _requests;

    public RequestList()
    {
        _requests = new LinkedList<Request>();
    }

    public bool AddItem(Request item)
    {
        try
        {
            _requests.AddLast(item);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool Update(Request new_item, Request old_item)
    {
        try
        {
            var node = _requests.Find(old_item);
            if (node != null)
            {
                _requests.Remove(node);
                _requests.AddLast(new_item);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public bool DeleteItem(Request item)
    {
        return _requests.Remove(item);
    }

    public List<Request> Get_Data()
    {
        return new List<Request>(_requests);
    }

    public bool IsEmpty()
    {
        return _requests.Count == 0;
    }

    public List<Request> Set_Data(List<Request> data)
    {
        _requests = new LinkedList<Request>(data);
        return Get_Data();
    }

    public Request Get_ById(int id)
    {
        return _requests.FirstOrDefault(r => r.Id == id);
    }
}