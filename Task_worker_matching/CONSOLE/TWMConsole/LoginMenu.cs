using Task_worker_matching.ServiceLayer;
using Task_worker_matching.Memory_Layer;

public class LoginMenu(AccountService service)
{
    private AccountService AS = service;
    private string? userType;
    private User user;

    public bool login()
    {
        Console.Out.WriteLine("Choose User Type:");
        Console.Out.WriteLine("1) Client");
        Console.Out.WriteLine("2) Worker");
        string? userType = Console.ReadLine();

        while (userType != "1" && userType != "2")
        {
            Console.Error.WriteLine("Error: Please Choose one of the 2 options");
            Console.Out.WriteLine("Choose User Type:");
            Console.Out.WriteLine("1) Client");
            Console.Out.WriteLine("2) Worker");
            userType = Console.ReadLine();
        }

        if (userType == "1")
        {
            user = new Client();
        }
        else
        {
            user = new Worker();
        }

        // Get Account Info
        Console.Out.WriteLine("Enter Your Email: ");
        string? email = Console.ReadLine();

        Console.Out.WriteLine("Enter Your Password: ");
        string? password = Console.ReadLine();

        // Assign common properties
        user.set_email(email ?? "");
        user.set_password(password ?? "");

        AccountValidation state = AS.login(user);
        switch (state)
        {
            case AccountValidation.AllCorrect:
                Console.Out.WriteLine($"Welcome {user.get_name}");
                return true;
            case AccountValidation.EmailWrong:
                Console.Error.WriteLine("Email is incorrect");
                return false;
            case AccountValidation.PasswordWrong:
                Console.Error.WriteLine("Password is incorrect");
                return false;
            case AccountValidation.UserWrong:
                Console.Error.WriteLine("Error: Something went wrong please try again");
                return false;
        }
        return false;
    }

}