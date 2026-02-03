using TaskManager.Core.Dtos;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services;

public class TaskService
{
    private readonly TaskRepository _repo;
    private int _nextId = 1;

    public TaskService(TaskRepository repo)
    {
        _repo = repo;
    }

    public int GenerateId() => _nextId++;

    public TaskItem CreateTask(TaskCreateDto dto)
    {
        var task = new TaskItem(GenerateId(), dto.Title, dto.Deadline)
        {
            Priority = dto.Priority,
            Notes = dto.Notes
        };

        _repo.Add(task);
        return task;
    }

    public void UpdateTask(TaskUpdateDto dto)
    {
        var existing = _repo.GetById(dto.Id);
        if (existing is not TaskItem task)
            throw new Exceptions.ValidationException("Task-ul nu există sau Id-ul nu este de tip Task.");

        task.Title = dto.Title;
        task.Deadline = dto.Deadline;
        task.Priority = dto.Priority;
        task.IsCompleted = dto.IsCompleted;
        task.Notes = dto.Notes;

        task.Validate();
    }

    public void ToggleComplete(int id)
    {
        var existing = _repo.GetById(id);
        if (existing is not TaskItem task)
            throw new Exceptions.ValidationException("Selectează un Task ca să-l marchezi complet/incomplet.");

        task.IsCompleted = !task.IsCompleted;
    }
}
