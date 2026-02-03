namespace TaskManager.Core.Exceptions;

public class ValidationException : TaskManagerException
{
    public ValidationException(string message) : base(message) { }
}
