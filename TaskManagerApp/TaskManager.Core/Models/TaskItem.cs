using TaskManager.Core.Interfaces;

namespace TaskManager.Core.Models;

public class TaskItem : WorkItem, IHasPriority
{
    public DateTime Deadline { get; set; }
    public bool IsCompleted { get; set; }
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

    public TaskItem(int id, string title, DateTime deadline)
        : base(id, title)
    {
        Deadline = deadline;
    }

    public override WorkItemType Type => WorkItemType.Task;

    public override string GetDetails()
        => $"{Title} | Deadline: {Deadline:yyyy-MM-dd} | Done: {IsCompleted} | Priority: {Priority}";

    public override void Validate()
    {
        base.Validate();
        if (Deadline < DateTime.Today.AddYears(-1))
            throw new Exceptions.ValidationException("Deadline invalid (prea vechi).");
    }
}
