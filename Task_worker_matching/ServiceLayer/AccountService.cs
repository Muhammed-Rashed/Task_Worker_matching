using System.Runtime.InteropServices;
using MyAvaloniaApp.Memory_Layer;
using PersistenceLayer;

namespace ServiceLayer;

public class AccountService
{
    private User currentUser;
    private Authenticator auth;
    private IUserRepository userRepo;

    public AccountValidation signup(User user)
    {
        AccountValidation state = auth.signup_authentication(user);
        if (state == AccountValidation.AllCorrect)
        {
            if (user is Client client)
            {
                userRepo = new ClientUserRepository();

                if (userRepo.add_user(user))
                    return AccountValidation.AllCorrect;

                return AccountValidation.EmailWrong;
            }
            else if (user is Worker worker)
            {
                userRepo = new WorkerUserRepository();

                if (userRepo.add_user(user))
                    return AccountValidation.AllCorrect;

                return AccountValidation.EmailWrong;
            }
        }
        return state;
    }

    // Give this method a User object with only email and password
    public AccountValidation login(User user)
    {
        AccountValidation state = auth.login_authentication(user);

        if (user is Client client)
        {
            userRepo = new ClientUserRepository();

            currentUser = userRepo.get_user(user.get_email());
        }
        else if (user is Worker worker)
        {
            userRepo = new WorkerUserRepository();

            currentUser = userRepo.get_user(user.get_email());
        }
        return state;
    }

    public bool update_user(User new_user)
    {
        if (currentUser is Client currentClient && new_user is Client new_client)
        {
            userRepo = new ClientUserRepository();


            currentUser = new_user;
            return userRepo.update_user(currentClient, new_client);
        }
        else if (currentUser is Worker currentWorker && new_user is Worker new_worker)
        {
            userRepo = new WorkerUserRepository();


            currentUser = new_user;
            return userRepo.update_user(currentWorker, new_worker);
        }
        return false;
    }

    public bool delete_user()
    {
        if (currentUser is Client)
        {
            userRepo = new ClientUserRepository();
            return userRepo.delete_user(currentUser);
        }
        else if (currentUser is Worker)
        {
            userRepo = new WorkerUserRepository();
            userRepo.delete_user(currentUser);
        }
        return false;
    }
}