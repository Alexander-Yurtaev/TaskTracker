using System.Text.Json;
using TaskTracker.Repository.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskTracker.Repository.Json;

public class TaskRepository : ITaskRepository
{
    private const string FileName = "TaskTracker.json";

    public async Task<List<Models.Task>> GetAllTasks()
    {
        await CreateIfNotExists();

        // Read from file
        List<TaskTracker.Repository.Models.Task> taskList = await LoadTasksAsync();

        return taskList;
    }

    public async Task<int> Add(string description)
    {
        description = description.Trim();
        if (string.IsNullOrEmpty(description))
        {
            throw new ArgumentException($"Description must not be empty!");
        }

        List<TaskTracker.Repository.Models.Task> taskList = await GetAllTasks();

        var id = taskList.Count == 0 ? 1 : taskList.Max(d => d.Id);
        TaskTracker.Repository.Models.Task task = new(id, description)
        {
            Status = TaskStatuses.ToDo,
            CreatedAt = DateTime.Now
        };

        taskList.Add(task);

        // Serialize and save
        await SaveTasksAsync(taskList);

        return task.Id;
    }

    public async Task Update(int id, string description)
    {
        List<TaskTracker.Repository.Models.Task> taskList = await GetAllTasks();

        TaskTracker.Repository.Models.Task? task = GetTaskById(id, taskList);

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
        List<TaskTracker.Repository.Models.Task> taskList = await GetAllTasks();
        taskList = taskList.Where(t => t.Status == status).ToList();

        return taskList;
    }

    public async Task Delete(int id)
    {
        List<TaskTracker.Repository.Models.Task> taskList = await GetAllTasks();

        TaskTracker.Repository.Models.Task? task = GetTaskById(id, taskList);

        if (task is null)
        {
            return;
        }

        taskList.Remove(task);

        await SaveTasksAsync(taskList);
    }

    public async Task UpdateStatus(int id, TaskStatuses status)
    {
        List<TaskTracker.Repository.Models.Task> taskList = await GetAllTasks();

        TaskTracker.Repository.Models.Task? task = GetTaskById(id, taskList);

        if (task is null)
        {
            throw new KeyNotFoundException($"Task with id={id} was not found.");
        }

        task.Status = status;
        task.UpdatedAt = DateTime.Now;

        await SaveTasksAsync(taskList);
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
            Console.WriteLine($"There is an exception while deserialize data from file: {e.Message}");
            return [];
        }
    }

    private async Task SaveTasksAsync(List<TaskTracker.Repository.Models.Task> taskList)
    {
        var contents = JsonSerializer.Serialize(taskList);
        await File.WriteAllTextAsync(FileName, contents);
    }

    private TaskTracker.Repository.Models.Task? GetTaskById(int id, List<TaskTracker.Repository.Models.Task> taskList)
    {
        if (id < 0)
        {
            throw new ArgumentException("ID must be positive");
        }

        return taskList.SingleOrDefault(t => t.Id == id);
    }
}