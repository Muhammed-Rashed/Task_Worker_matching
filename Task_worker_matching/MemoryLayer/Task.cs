using System.Collections.Generic;

namespace Task_worker_matching.Memory_Layer;

public class Task
{
    public string Name { get; set; }
    public int Id { get; set; }
    public List<string> Specialty { get; set; }
    public double Avg_Time { get; set; }
    public decimal AVG_Fee { get; set; }
}