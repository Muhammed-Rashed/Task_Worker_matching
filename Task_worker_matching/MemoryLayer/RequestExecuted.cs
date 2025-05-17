using System;
using MyAvaloniaApp.Memory_Layer;

namespace MyAvaloniaApp.Memory_Layer;

public class RequestExecuted
{
    private Request request;
    private Worker worker;
    private DateTime actual_start_time;
    private DateTime actual_end_time;
    private RequestStatus status;
    private double client_rate;
    private double worker_rate;
    private string client_feedback;
    private string worker_feedback;
    private int request_id;

    // Setters
    public void SetRequest(Request value) => request = value;
    public void SetWroker(Worker value) => worker = value;
    public void SetActualStartTime(DateTime value) => actual_start_time = value;
    public void SetActualEndTime(DateTime value) => actual_end_time = value;
    public void SetStatus(RequestStatus value) => status = value;
    public void SetClientRate(double value) => client_rate = value;
    public void SetWorkerRate(double value) => worker_rate = value;
    public void SetClientFeedback(string value) => client_feedback = value;
    public void SetWorkerFeedback(string value) => worker_feedback = value;

    // Getters
    public Request GetRequest() => request;
    public Worker GetWorker() => worker;
    public DateTime GetActalStartTime() => actual_start_time;
    public DateTime GetActalEndTime() => actual_end_time;
    public RequestStatus GetStatus() => status;
    public double GetClientRate() => client_rate;
    public double GetWorkerRate() => worker_rate;
    public string GetClientFeedback() => client_feedback;
    public string GetWorkerFeedback() => worker_feedback;
    public int GetId() => this.request_id;
}