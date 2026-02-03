using TaskManager.Core.Models;

namespace TaskManager.Core.Dtos;

public class TaskCreateDto
{
    public string Title { get; set; } = "";
    public DateTime Deadline { get; set; }
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public string? Notes { get; set; }
}
