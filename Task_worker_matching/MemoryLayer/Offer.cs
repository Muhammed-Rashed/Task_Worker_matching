using System;

namespace MyAvaloniaApp.Memory_Layer;

public class Offer
{
    private int id;

    // Pointer to Worker and Request using unsafe code
    private Worker worker;
    private Request request;

    private DateTime expirationTime;
    private TimeSpan time;
    private double fee;
    private string message;

    // Getters and Setters
    public int GetId() => id;
    public void SetId(int value) => id = value;

    public Worker GetWorker() => worker;
    public void SetWorker(Worker value) => worker = value;

    public Request GetRequest() => request;
    public void SetRequest(Request value) => request = value;

    public DateTime GetExpirationTime() => expirationTime;
    public void SetExpirationTime(DateTime value) => expirationTime = value;

    public TimeSpan GetTime() => time;
    public void SetTime(TimeSpan value) => time = value;

    public double GetFee() => fee;
    public void SetFee(double value) => fee = value;

    public string GetMessage() => message;
    public void SetMessage(string value) => message = value;
}