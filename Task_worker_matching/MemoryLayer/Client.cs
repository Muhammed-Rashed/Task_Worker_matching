public class Client : User
{
    private string payment_info;
    private string address;
    private double avg_rating;

    public Client(string payment_info, string address)
    {
        this.payment_info = payment_info;
        this.address = address;
        this.avg_rating = 0.0;
    }

    public string get_payment_info() => payment_info;
    public string get_address() => address;
    public double get_avg_rating() => avg_rating;

    public void set_payment_info(string payment_info)
    {
        this.payment_info = payment_info;
    }

    public void set_address(string address)
    {
        this.address = address;
    }

    public void set_overall_rating(double rating)
    {
        this.avg_rating = rating;
    }
}
