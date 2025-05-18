namespace Task_worker_matching.Memory_Layer;

public class User
{
    private string name;
    private string email;
    private string phone_number;
    private string password;
    private int user_ID;
    private double overall_rating;

    public string get_name() => name;
    public string get_email() => email;
    public string get_phone_number() => phone_number;
    public string get_password() => password;
    public int get_user_ID() => user_ID;
    public double get_overall_rating() => overall_rating;

    public void set_name(string name) => this.name = name;
    public void set_email(string email) => this.email = email;
    public void set_phone_number(string phone) => this.phone_number = phone;
    public void set_password(string password) => this.password = password;
    public void set_user_ID(int id) => this.user_ID = id;
    public void set_overall_rating(double rating) => this.overall_rating = rating;

}
