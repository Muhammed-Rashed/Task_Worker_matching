using System.Collections.Generic;

namespace PersistenceLayer
{
    public interface IRepositoryStrategy<T>
    {
        bool add_item(T item);
        List<T> get_items(int user_id);
        bool update_item(T new_item, T old_item);
        bool delete_item(T item_id);
    }
}