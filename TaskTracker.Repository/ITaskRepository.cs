namespace TaskTracker.Repository;

public interface ITaskRepository
{
    Task<int> Add(string description);

    Task<List<TaskTracker.Repository.Models.Task>> GetAllTasks();

    Task Update(int id, string description);
}