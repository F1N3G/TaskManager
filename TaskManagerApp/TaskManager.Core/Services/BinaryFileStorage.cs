using System.Text.Json;
using TaskManager.Core.Exceptions;
using TaskManager.Core.Models;

namespace TaskManager.Core.Storage;

public static class BinaryFileStorage
{
    // DTO comun pentru ambele tipuri (Task/Reminder)
    private class WorkItemBinDto
    {
        public string Type { get; set; } = "";

        public int Id { get; set; }
        public string Title { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string? Notes { get; set; }

        // Task fields
        public DateTime? Deadline { get; set; }
        public bool? IsCompleted { get; set; }
        public int? Priority { get; set; }

        // Reminder fields
        public DateTime? RemindAt { get; set; }
    }

    public static void Save(string path, IEnumerable<WorkItem> items)
    {
        try
        {
            var payload = items.Select(i =>
            {
                if (i is TaskItem t)
                {
                    return new WorkItemBinDto
                    {
                        Type = "Task",
                        Id = t.Id,
                        Title = t.Title,
                        CreatedAt = t.CreatedAt,
                        Notes = t.Notes,
                        Deadline = t.Deadline,
                        IsCompleted = t.IsCompleted,
                        Priority = (int)t.Priority
                    };
                }

                if (i is ReminderItem r)
                {
                    return new WorkItemBinDto
                    {
                        Type = "Reminder",
                        Id = r.Id,
                        Title = r.Title,
                        CreatedAt = r.CreatedAt,
                        Notes = r.Notes,
                        RemindAt = r.RemindAt
                    };
                }

                // fallback (nu ar trebui să ajungă aici)
                return new WorkItemBinDto
                {
                    Type = "Unknown",
                    Id = i.Id,
                    Title = i.Title,
                    CreatedAt = i.CreatedAt,
                    Notes = i.Notes
                };
            }).ToList();

            var bytes = JsonSerializer.SerializeToUtf8Bytes(payload);
            File.WriteAllBytes(path, bytes);
        }
        catch (Exception ex)
        {
            throw new StorageException("Eroare la salvarea în fișier binar.", ex);
        }
    }

    public static List<WorkItem> Load(string path)
    {
        try
        {
            var list = new List<WorkItem>();
            if (!File.Exists(path)) return list;

            var bytes = File.ReadAllBytes(path);
            var payload = JsonSerializer.Deserialize<List<WorkItemBinDto>>(bytes) ?? new();

            foreach (var el in payload)
            {
                if (el.Type == "Task")
                {
                    var deadline = el.Deadline ?? DateTime.Now;
                    var t = new TaskItem(el.Id, el.Title, deadline)
                    {
                        CreatedAt = el.CreatedAt,
                        Notes = el.Notes,
                        IsCompleted = el.IsCompleted ?? false,
                        Priority = (PriorityLevel)(el.Priority ?? (int)PriorityLevel.Medium)
                    };
                    list.Add(t);
                }
                else if (el.Type == "Reminder")
                {
                    var remindAt = el.RemindAt ?? DateTime.Now;
                    var r = new ReminderItem(el.Id, el.Title, remindAt)
                    {
                        CreatedAt = el.CreatedAt,
                        Notes = el.Notes
                    };
                    list.Add(r);
                }
            }

            return list;
        }
        catch (Exception ex)
        {
            throw new StorageException("Eroare la încărcarea din fișier binar.", ex);
        }
    }
}
