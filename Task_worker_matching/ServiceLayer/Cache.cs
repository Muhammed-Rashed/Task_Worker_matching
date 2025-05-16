/*using System;
using System.Collections.Generic;
using PersistenceLayer;
using MyAvaloniaApp.Memory_Layer;

class Cache<T>
{
    private IMemory<T> memory;
    private IRepositryStrategy<T> repositry;
    private IRepositryFactory<T> factory;

    public bool add(T item)
    {
        return memory.add(item) && IRepositryStrategy.add(item) && IRepositryFactory.add(item);
    }

    public bool update(T new_item, T old_item)
    {
        return memory.update(T new_item, T old_item) &&
               IRepositryStrategy.update(T new_item, T old_item) &&
               IRepositryFactory.update(T new_item, T old_item);
    }

    public bool delete(T item)
    {
        return memory.delete(item) && IRepositryStrategy.delete(item) && IRepositryFactory.delete(item);
    }

    public List<T> get_data()
    {
        if (!memory.isEmpty())
            return memory.get_data();

        if (!repositry.isEmpty())
        {
            memory.set_data(repositry.get_data());
            return repositry.get_data();
        }

        if (!factory.isEmpty())
        {
            memory.set_data(factory.get_data());
            repositry.set_date(factory.get_data());
            return repositry.get_data();
        }

        return null;
    }
}*/