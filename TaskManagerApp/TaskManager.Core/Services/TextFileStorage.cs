using TaskManager.Core.Exceptions;
using TaskManager.Core.Models;

namespace TaskManager.Core.Storage;

public static class TextFileStorage
{
    // Format simplu: Type|Id|Title|CreatedAt|Notes|Deadline|IsCompleted|Priority|RemindAt
    public static void Save(string path, IEnumerable<WorkItem> items)
    {
        try
        {
            var lines = items.Select(i =>
            {
                if (i is TaskItem t)
                    return $"Task|{t.Id}|{Escape(t.Title)}|{t.CreatedAt:o}|{Escape(t.Notes)}|{t.Deadline:o}|{t.IsCompleted}|{(int)t.Priority}|";
                if (i is ReminderItem r)
                    return $"Reminder|{r.Id}|{Escape(r.Title)}|{r.CreatedAt:o}|{Escape(r.Notes)}||||{r.RemindAt:o}";
                return "";
            }).Where(x => !string.IsNullOrWhiteSpace(x));

            File.WriteAllLines(path, lines);
        }
        catch (Exception ex)
        {
            throw new StorageException("Eroare la salvarea în fișier text.", ex);
        }
    }

    public static List<WorkItem> Load(string path)
    {
        try
        {
            var list = new List<WorkItem>();
            if (!File.Exists(path)) return list;

            foreach (var line in File.ReadAllLines(path))
            {
                var p = line.Split('|');
                var type = p[0];

                if (type == "Task")
                {
                    var id = int.Parse(p[1]);
                    var title = Unescape(p[2]);
                    var created = DateTime.Parse(p[3]);
                    var notes = Unescape(p[4]);
                    var deadline = DateTime.Parse(p[5]);
                    var done = bool.Parse(p[6]);
                    var priority = (PriorityLevel)int.Parse(p[7]);

                    var t = new TaskItem(id, title, deadline)
                    {
                        CreatedAt = created,
                        Notes = notes,
                        IsCompleted = done,
                        Priority = priority
                    };
                    list.Add(t);
                }
                else if (type == "Reminder")
                {
                    var id = int.Parse(p[1]);
                    var title = Unescape(p[2]);
                    var created = DateTime.Parse(p[3]);
                    var notes = Unescape(p[4]);
                    var remindAt = DateTime.Parse(p[8]);

                    var r = new ReminderItem(id, title, remindAt)
                    {
                        CreatedAt = created,
                        Notes = notes
                    };
                    list.Add(r);
                }
            }

            return list;
        }
        catch (Exception ex)
        {
            throw new StorageException("Eroare la încărcarea din fișier text.", ex);
        }
    }

    private static string Escape(string? s) => (s ?? "").Replace("\r", " ").Replace("\n", " ").Replace("|", "¦");
    private static string Unescape(string? s) => (s ?? "").Replace("¦", "|");
}
