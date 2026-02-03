namespace TaskManager.Core.Interfaces;

public interface IStorable<T>
{
    void Save(string path, IEnumerable<T> items);
    List<T> Load(string path);
}
