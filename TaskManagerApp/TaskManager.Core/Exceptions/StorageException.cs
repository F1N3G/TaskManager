namespace TaskManager.Core.Exceptions;

public class StorageException : TaskManagerException
{
    public StorageException(string message) : base(message) { }
    public StorageException(string message, Exception inner) : base(message, inner) { }
}
