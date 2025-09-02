using TaskTracker.Repository.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskTracker.Repository;

public interface ITaskRepository
{
    Task<int> Add(string description);

    Task<List<TaskTracker.Repository.Models.Task>> GetAllTasks();

    Task Update(int id, string description);

    Task<List<TaskTracker.Repository.Models.Task>> GetTasks(Models.TaskStatuses status);

    Task Delete(int id);

    Task UpdateStatus(int id, TaskStatuses status);
}