using TaskManager.Core.Models;

namespace TaskManager.Core.Dtos;

public class TaskUpdateDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public DateTime Deadline { get; set; }
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public bool IsCompleted { get; set; }
    public string? Notes { get; set; }
}
