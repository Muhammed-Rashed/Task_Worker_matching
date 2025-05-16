using System.Collections.Generic;

namespace PersistenceLayer;

public interface IRepositoryStrategy<T>
{
    bool add_item(int user_id, int new_item_id, T item);
    List<T> get_items(int user_id);
    bool update_item(int user_id, int new_item, T old_item);
    bool delete_item(int user_id, int item_id);
}