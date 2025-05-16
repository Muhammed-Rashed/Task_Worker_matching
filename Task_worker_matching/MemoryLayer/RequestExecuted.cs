using System;
using Task_worker_matching.Memory_Layer;

public class RequestExecuted
{
    private Offer offer;
    private Request request;
    private DateTime actal_start_time;
    private DateTime actal_end_time;
    private RequestStatus status;
    private double client_rate;
    private double worker_rate;
    private string client_feedback;
    private string worker_feedbaack;

    // Setters
    public void SetOffer(Offer value) => offer = value;
    public void SetRequest(Request value) => request = value;
    public void SetActalStartTime(DateTime value) => actal_start_time = value;
    public void SetActalEndTime(DateTime value) => actal_end_time = value;
    public void SetStatus(RequestStatus value) => status = value;
    public void SetClientRate(double value) => client_rate = value;
    public void SetWorkerRate(double value) => worker_rate = value;
    public void SetClientFeedback(string value) => client_feedback = value;
    public void SetWorkerFeedback(string value) => worker_feedbaack = value;

    // Getters
    public Offer GetOffer() => offer;
    public Request GetRequest() => request;
    public DateTime GetActalStartTime() => actal_start_time;
    public DateTime GetActalEndTime() => actal_end_time;
    public RequestStatus GetStatus() => status;
    public double GetClientRate() => client_rate;
    public double GetWorkerRate() => worker_rate;
    public string GetClientFeedback() => client_feedback;
    public string GetWorkerFeedback() => worker_feedbaack;

    public int GetId()
    {
        throw new NotImplementedException();
    }
}