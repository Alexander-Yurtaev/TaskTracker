using System.Text.Json;
using TaskTracker.Repository.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskTracker.Repository.Json;

public class TaskRepository : ITaskRepository
{
    private const string FileName = "TaskTracker.json";

    public async Task<int> Add(string description)
    {
        await CreateIfNotExists();

        // Read from file
        List<TaskTracker.Repository.Models.Task> taskList = await LoadTasksAsync();

        TaskTracker.Repository.Models.Task task = new()
        {
            Id = taskList.Count == 0 ? 1 : taskList.Max(d => d.Id),
            Description = description,
            Status = TaskStatuses.ToDo,
            CreatedAt = DateTime.Now
        };

        taskList.Add(task);

        // Serialize and save
        await SaveTasksAsync(taskList);

        return task.Id;
    }

    public async Task<List<Models.Task>> GetAllTasks()
    {
        await CreateIfNotExists();

        // Read from file
        List<TaskTracker.Repository.Models.Task> taskList = await LoadTasksAsync();

        return taskList;
    }

    public async Task Update(int id, string description)
    {
        await CreateIfNotExists();

        // Read from file
        List<TaskTracker.Repository.Models.Task> taskList = await LoadTasksAsync();

        TaskTracker.Repository.Models.Task? task = taskList.FirstOrDefault(t => t.Id == id);

        if (task is null)
        {
            throw new NullReferenceException($"Task with id={id} was not found.");
        }

        task.Description = description;
        task.UpdatedAt = DateTime.Now;

        await SaveTasksAsync(taskList);
    }

    public async Task<List<Models.Task>> GetTasks(TaskStatuses status)
    {
        await CreateIfNotExists();

        // Read from file
        List<TaskTracker.Repository.Models.Task> taskList = await LoadTasksAsync();
        taskList = taskList.Where(t => t.Status == status).ToList();

        return taskList;
    }

    private async Task CreateIfNotExists()
    {
        if (!File.Exists(FileName))
        {
            // Create empty list
            var emptyArray = new List<TaskTracker.Repository.Models.Task>();

            // Serialize and save
            await SaveTasksAsync(emptyArray);
        }
    }

    private async Task<List<TaskTracker.Repository.Models.Task>> LoadTasksAsync()
    {
        // Read from file
        string jsonString = await File.ReadAllTextAsync(FileName);

        // Deserialize into Task
        try
        {
            List<Models.Task> taskList =
                JsonSerializer.Deserialize<List<Models.Task>>(jsonString) ?? [];

            return taskList;
        }
        catch (JsonException e)
        {
            return [];
        }
    }

    private async Task SaveTasksAsync(List<TaskTracker.Repository.Models.Task> taskList)
    {
        var contents = JsonSerializer.Serialize(taskList);
        await File.WriteAllTextAsync(FileName, contents);
    }
}