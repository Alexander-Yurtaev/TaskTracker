namespace TaskTracker.Repository;

public interface ITaskRepository
{
    Task<int> Add(string description);
}