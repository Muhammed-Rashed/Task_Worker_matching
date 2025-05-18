using Task_worker_matching.Memory_Layer;
using Task_worker_matching.Views;
using PersistenceLayer;

namespace Task_worker_matching.ServiceLayer;

public class Authenticator
{
    private IUserRepository userRepo;
    public AccountValidation login_authentication(User user)
    {
        if (user is Client client)
        {
            userRepo = new ClientUserRepository();

            Client foundClient = (Client)userRepo.get_user(client.get_email());

            if (foundClient != null)
            {
                if (foundClient.get_password() == client.get_password())
                {
                    return AccountValidation.AllCorrect;
                }
                else
                {
                    return AccountValidation.PasswordWrong;
                }
            }
            else
            {
                return AccountValidation.EmailWrong;
            }
        }
        else if (user is Worker worker)
        {
            userRepo = new WorkerUserRepository();

            Worker foundWorker = (Worker)userRepo.get_user(worker.get_email());

            if (foundWorker != null)
            {
                if (foundWorker.get_password() == worker.get_password())
                {
                    return AccountValidation.AllCorrect;
                }
                else
                {
                    return AccountValidation.PasswordWrong;
                }
            }
            else
            {
                return AccountValidation.EmailWrong;
            }
        }
        return AccountValidation.UserWrong;
    }

    public AccountValidation signup_authentication(User user)
    {
        if (user is Client client)
        {
            userRepo = new ClientUserRepository();

            Client foundClient = (Client)userRepo.get_user(client.get_email());

            if (foundClient == null)
            {
                if (userRepo.add_user(user))
                    return AccountValidation.AllCorrect;

                return AccountValidation.EmailWrong;
            }
            else
            {
                return AccountValidation.EmailWrong;
            }
        }
        else if (user is Worker worker)
        {
            userRepo = new WorkerUserRepository();

            Worker foundWorker = (Worker)userRepo.get_user(worker.get_email());

            if (foundWorker == null)
            {
                if (userRepo.add_user(user))
                    return AccountValidation.AllCorrect;

                return AccountValidation.EmailWrong;
            }
            else
            {
                return AccountValidation.EmailWrong;
            }
        }
        return AccountValidation.UserWrong;
    }
}