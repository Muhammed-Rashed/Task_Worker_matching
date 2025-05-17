namespace MyAvaloniaApp.Memory_Layer;

public class Client : User
{
    private string payment_info;
    private string address;

    public string get_payment_info() => payment_info;
    public string get_address() => address;

    public void set_payment_info(string payment_info)
    {
        this.payment_info = payment_info;
    }

    public void set_address(string address)
    {
        this.address = address;
    }

}
