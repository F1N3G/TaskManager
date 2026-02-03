namespace TaskManager.Core.Models;

public class ReminderItem : WorkItem
{
    public DateTime RemindAt { get; set; }

    public ReminderItem(int id, string title, DateTime remindAt)
        : base(id, title)
    {
        RemindAt = remindAt;
    }

    public override WorkItemType Type => WorkItemType.Reminder;

    public override string GetDetails()
        => $"{Title} | Remind at: {RemindAt:yyyy-MM-dd HH:mm}";
}
