using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces;

public interface IHasPriority
{
    PriorityLevel Priority { get; set; }
}
