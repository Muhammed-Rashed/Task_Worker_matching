using System;
using System.Collections.Generic;
namespace Task_worker_matching.Memory_Layer;

public class Worker
{
    private int id;
    private string available_locations;
    private string available_time;
    private LinkedList<string> speciality = new LinkedList<string>();
    private List<RequestExecuted> requestsExecuted = new List<RequestExecuted>();
    
    public void SetId(int id) => this.id = id;
    public int GetId() => this.id;

    // Setter for available_locations
    public void SetAvailableLocations(string available_locations)
    {
        this.available_locations = available_locations;
    }

    // Getter for available_locations
    public string GetAvailableLocations()
    {
        return this.available_locations;
    }

    // Setter for available_time
    public void SetAvailableTime(string available_time)
    {
        this.available_time = available_time;
    }

    // Getter for available_time
    public string GetAvailableTime()
    {
        return this.available_time;
    }

    // Setter for speciality
    public void SetSpeciality(LinkedList<string> speciality)
    {
        this.speciality = speciality;
    }

    // Getter for speciality
    public LinkedList<string> GetSpeciality()
    {
        return this.speciality;
    }

    // Setter for requestsExecuted
    public void SetRequestsExecuted(List<RequestExecuted> requestsExecuted)
    {
        this.requestsExecuted = requestsExecuted;
    }

    // Getter for requestsExecuted
    public List<RequestExecuted> GetRequestsExecuted()
    {
        return this.requestsExecuted;
    }
}