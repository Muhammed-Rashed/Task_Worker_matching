using System;

public class Offer
{
    private int id;
    
    // Pointer to Worker and Request using unsafe code
    private unsafe Worker* worker;    
    private unsafe Request* request; 
    
    private DateTime expirationTime;
    private TimeSpan time;
    private double fee;
    private string message;

    // Getters and Setters for the id
    public int GetId() => id;
    public void SetId(int value) => id = value;

    // Getters and Setters for the Worker pointer
    public unsafe Worker* GetWorker() => worker;
    public unsafe void SetWorker(Worker* value) => worker = value;

    // Getters and Setters for the Request pointer
    public unsafe Request* GetRequest() => request;
    public unsafe void SetRequest(Request* value) => request = value;

    // Getters and Setters for the expiration time
    public DateTime GetExpirationTime() => expirationTime;
    public void SetExpirationTime(DateTime value) => expirationTime = value;

    // Getters and Setters for the time
    public TimeSpan GetTime() => time;
    public void SetTime(TimeSpan value) => time = value;

    // Getters and Setters for the fee
    public double GetFee() => fee;
    public void SetFee(double value) => fee = value;

    // Getters and Setters for the message
    public string GetMessage() => message;
    public void SetMessage(string value) => message = value;
}
