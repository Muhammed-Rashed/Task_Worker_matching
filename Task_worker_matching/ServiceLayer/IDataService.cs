using System;
using System.Collections.Generic;

namespace Task_worker_matching.ServiceLayer;
public interface IDataService<T>
{
    bool add(T item);
    bool update(T new_item, T old_item);
    bool delete(T item);
    List<T> get_data();
}
