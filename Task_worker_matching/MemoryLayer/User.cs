public class User
{
    private string name;
    private string email;
    private string phone_number;
    private string password;
    private string user_ID;
    private double overall_rating;

    public User(string userID, string name, string email, string phone, string password)
    {
        this.user_ID = userID;
        this.name = name;
        this.email = email;
        this.phone_number = phone;
        this.password = password;
        this.overall_rating = 0.0;
    }

    public string get_name() => name;
    public string get_email() => email;
    public string get_phone_number() => phone_number;
    public string get_password() => password;
    public string get_user_ID() => user_ID;
    public double get_overall_rating() => overall_rating;

    public void set_name(string name) => this.name = name;
    public void set_email(string email) => this.email = email;
    public void set_phone_number(string phone) => this.phone_number = phone;
    public void set_password(string password) => this.password = password;
    public void set_user_ID(string id) => this.user_ID = id;
    public void set_overall_rating(double rating) => this.overall_rating = rating;
    
}
