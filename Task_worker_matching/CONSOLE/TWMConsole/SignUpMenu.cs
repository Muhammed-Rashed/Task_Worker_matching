using Task_worker_matching.ServiceLayer;
using Task_worker_matching.Memory_Layer;

public class SignUpMenu(AccountService service)
{
    private AccountService AS = service;
    private string? userType;
    private User user;

    public bool signup()
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
        Console.Out.WriteLine("Enter Your name: ");
        string? name = Console.ReadLine();

        Console.Out.WriteLine("Enter Your Email: ");
        string? email = Console.ReadLine();

        Console.Out.WriteLine("Enter Your Password: ");
        string? password = Console.ReadLine();

        Console.Out.WriteLine("Enter Your Phone Number: ");
        string? phoneNumber = Console.ReadLine();

        // Assign common properties
        user.set_name(name ?? "");
        user.set_email(email ?? "");
        user.set_password(password ?? "");
        user.set_phone_number(phoneNumber ?? "");

        if (userType == "1") // Client
        {
            Console.Out.WriteLine("Enter Your Address: ");
            string? address = Console.ReadLine();
            Console.Out.WriteLine("Enter Your Payment Information (Card number or PayPal ID): ");
            string? paymentInfo = Console.ReadLine();

            ((Client)user).set_address(address ?? "");
            ((Client)user).set_payment_info(paymentInfo ?? "");
        }
        else // Worker
        {
            // Specialities
            Console.Out.WriteLine("Enter your Specialities (comma-separated): ");
            string? specialitiesInput = Console.ReadLine() ?? "";
            List<string> specialities = new List<string>();
            foreach (var spec in specialitiesInput.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                specialities.Add(spec.Trim());
            }
            ((Worker)user).SetSpecialities(specialities);

            // Available locations
            Console.Out.WriteLine("Enter your Available Locations (comma-separated): ");
            string? locationsInput = Console.ReadLine() ?? "";
            ((Worker)user).SetAvailableLocations(locationsInput);

            // Available work time
            TimeSpan startTime;
            TimeSpan endTime;

            while (true)
            {
                Console.Out.WriteLine("Enter Available Start Time (HH:mm): ");
                string? startInput = Console.ReadLine();
                if (TimeSpan.TryParse(startInput, out startTime))
                    break;
                Console.Error.WriteLine("Invalid time format. Please enter in HH:mm format.");
            }

            while (true)
            {
                Console.Out.WriteLine("Enter Available End Time (HH:mm): ");
                string? endInput = Console.ReadLine();
                if (TimeSpan.TryParse(endInput, out endTime))
                {
                    if (endTime > startTime)
                        break;
                    else
                        Console.Error.WriteLine("End time must be after start time.");
                }
                else
                {
                    Console.Error.WriteLine("Invalid time format. Please enter in HH:mm format.");
                }
            }

            ((Worker)user).SetAvailableStartTime(startTime);
            ((Worker)user).SetAvailableEndTime(endTime);
        }

        AccountValidation state = AS.signup(user);
        switch (state)
        {
            case AccountValidation.AllCorrect:
                Console.Out.WriteLine($"Account Created successfully, Welcome {user.get_name()}");
                return true;
            case AccountValidation.EmailWrong:
                Console.Error.WriteLine("Email already exists or has incorrect format");
                return false;
            case AccountValidation.PasswordWrong:
                Console.Error.WriteLine("Password has incorrect format");
                return false;
            case AccountValidation.UserWrong:
                Console.Error.WriteLine("Error: Something went wrong please try again");
                return false;
        }
        return false;
    }

}