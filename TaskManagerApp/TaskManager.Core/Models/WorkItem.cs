using TaskManager.Core.Interfaces;

namespace TaskManager.Core.Models;

public abstract class WorkItem : IValidatable
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    protected WorkItem(int id, string title)
    {
        Id = id;
        Title = title;
        CreatedAt = DateTime.Now;
    }

    public abstract WorkItemType Type { get; }

    public abstract string GetDetails();

    public virtual void Validate()
    {
        if (Id <= 0) throw new Exceptions.ValidationException("Id trebuie să fie > 0.");
        if (string.IsNullOrWhiteSpace(Title)) throw new Exceptions.ValidationException("Titlul nu poate fi gol.");
        if (Title.Length > 100) throw new Exceptions.ValidationException("Titlul e prea lung (max 100).");
    }
}
