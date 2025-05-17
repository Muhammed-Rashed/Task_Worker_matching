using System;
using System.Collections.Generic;
namespace MyAvaloniaApp.Memory_Layer;

public class Worker : User
{
    private int id;
    private string available_locations;
    private TimeSpan available_start_time;
    private TimeSpan available_end_time;
    private List<string> specialities = new List<string>();
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

    // Setter for available_start_time
    public void SetAvailableStartTime(TimeSpan available_start_time)
    {
        this.available_start_time = available_start_time;
    }

    // Getter for available_start_time
    public TimeSpan GetAvailableStartTime()
    {
        return this.available_start_time;
    }

    // Setter for available_end_time
    public void SetAvailableEndTime(TimeSpan available_end_time)
    {
        this.available_end_time = available_end_time;
    }

    // Getter for available_start_time
    public TimeSpan GetAvailableEndTime()
    {
        return this.available_end_time;
    }

    // Setter for speciality
    public void SetSpecialities(List<string> specialities)
    {
        this.specialities = specialities;
    }

    // Getter for speciality
    public List<string> GetSpecialities()
    {
        return this.specialities;
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