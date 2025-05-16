using System.Collections.Generic;

namespace Task_worker_matching.Memory_Layer;

public interface IMemory<T>
{
    bool AddItem(T item);
    bool Update(T new_item, T old_item);
    bool DeleteItem(T item);
    List<T> Get_Data();
    List<T> Set_Data(List<T> data);
    bool IsEmpty();
}