using System;
using System.Collections.Generic;

namespace MyAvaloniaApp.Memory_Layer;

class RequestExecutedList
{
    private List<RequestExecuted> requestExecuted = new List<RequestExecuted>();

    public bool Add(RequestExecuted item)
    {
        if (item != null)
        {
            requestExecuted.Add(item);
            return true;
        }
        return false;
    }

    public bool Update(RequestExecuted newItem, RequestExecuted oldItem)
    {
        int index = requestExecuted.IndexOf(oldItem);
        if (index != -1)
        {
            requestExecuted[index] = newItem;
            return true;
        }
        return false;
    }

    public bool Delete(RequestExecuted item)
    {
        if (item != null)
        {
            return requestExecuted.Remove(item);
        }
        return false;
    }

    public List<RequestExecuted> GetData()
    {
        return requestExecuted;
    }

    public void SetData(List<RequestExecuted> newList)
    {
        if (newList != null)
            requestExecuted = newList;
    }

    public bool IsEmpty()
    {
        return requestExecuted.Count == 0;
    }

    public RequestExecuted GetById(int id)
    {
        foreach (var req in requestExecuted)
        {
            if (req.GetId() == id)
                return req;
        }
        return null;
    }
}