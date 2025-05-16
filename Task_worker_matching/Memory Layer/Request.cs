using System;

namespace Task_worker_matching.Memory_Layer;

public class Request
{
    public string Description { get; set; }
    public DateTime PreferredDate { get; set; }
    public RequestStatus Status { get; set; }
    public string Address { get; set; }
    public string Location { get; set; }
    public DateTime PlacementTime { get; set; }
    public bool IsPrivate { get; set; } // If public request this is false
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int TaskId { get; set; }
    public Task Task { get; set; }

    public void SetStatus(RequestStatus status)
    {
        Status = status; // Very important
    }
}