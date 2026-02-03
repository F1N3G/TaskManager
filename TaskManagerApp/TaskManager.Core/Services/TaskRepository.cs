using TaskManager.Core.Models;

namespace TaskManager.Core.Services;

public class TaskRepository
{
    private readonly List<WorkItem> _items = new();

    public IReadOnlyList<WorkItem> Items => _items;

    public void Add(WorkItem item)
    {
        item.Validate();
        if (_items.Any(x => x.Id == item.Id))
            throw new Exceptions.ValidationException("Există deja un item cu același Id.");

        _items.Add(item);
    }

    public bool Remove(int id)
        => _items.RemoveAll(x => x.Id == id) > 0;

    public WorkItem? GetById(int id)
        => _items.FirstOrDefault(x => x.Id == id);

    public IEnumerable<TaskItem> TasksOnly()
        => _items.OfType<TaskItem>();

    public IEnumerable<WorkItem> Search(string text)
        => _items.Where(i => i.Title.Contains(text, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<TaskItem> SortTasksByDeadline()
        => TasksOnly().OrderBy(t => t.Deadline);

    public IEnumerable<TaskItem> FilterTasks(Func<TaskItem, bool> predicate)
        => TasksOnly().Where(predicate);
}
