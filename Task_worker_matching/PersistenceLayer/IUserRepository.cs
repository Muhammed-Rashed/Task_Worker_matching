using System.Collections.Generic;
using Task_worker_matching.Memory_Layer;

namespace PersistenceLayer
{
    public interface IUserRepository
    {
        public bool add_user(User new_user);
        public User get_user(int userID);
        public List<User> get_all_users();
        public bool update_user(User new_user, User old_user);
        public bool delete_user(User user);
    }
}