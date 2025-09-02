using FluentAssertions;
using System.Text.Json;
using TaskTracker.Repository.Json;
using TaskTracker.Repository.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskTracker.Tests.Json;

public class TaskRepositoryTests
{
    private const string FileName = "TaskTracker.json";

    [Fact]
    public async Task AddTest()
    {
        // Arrange
        if (File.Exists(FileName))
        {
            File.Delete(FileName);
        }

        // Act
        TaskRepository repository = new();
        var id = await repository.Add("New Test Task");

        // Assert
        id.Should().BeGreaterThan(0, "Id was not initial");
        File.Exists(FileName).Should().BeTrue("File was not created.");

        string jsonString = await File.ReadAllTextAsync(FileName);
        List<TaskTracker.Repository.Models.Task>? taskList = JsonSerializer.Deserialize<List<TaskTracker.Repository.Models.Task>>(jsonString);

        taskList.Should().NotBeNull();
        taskList.Count.Should().Be(1);
    }

    [Fact]
    public async Task GetAllTasksTest()
    {
        // Arrange
        if (File.Exists(FileName))
        {
            File.Delete(FileName);
        }

        List<TaskTracker.Repository.Models.Task> taskList = new();
        for (int i = 1; i < 11; i++)
        {
            TaskTracker.Repository.Models.Task task = CreateTask(i, $"Test task for GetAllTasks method #{i}.");
            taskList.Add(task);
        }
        await SaveTasksAsync(taskList);

        // Act
        TaskRepository repository = new();
        var tasks = await repository.GetAllTasks();

        // Assert
        tasks.Should().NotBeNullOrEmpty();
        tasks.Count.Should().Be(10);

        foreach (var task in tasks)
        {
            task.Description.Should().StartWith("Test task for GetAllTasks method #");
        }
    }

    [Fact]
    public async Task GetTasksTest()
    {
        // Arrange
        if (File.Exists(FileName))
        {
            File.Delete(FileName);
        }

        List<TaskTracker.Repository.Models.Task> taskList = new();
        foreach (var status in Enum.GetValues<TaskStatuses>())
        {
            for (int i = 1; i < 11; i++)
            {
                int id = i + (int)status;
                TaskTracker.Repository.Models.Task task = CreateTask(i, $"Test task for GetAllTasks method #{id}.");
                task.Status = status;
                taskList.Add(task);
            }
        }
        
        await SaveTasksAsync(taskList);

        // Act
        Dictionary<TaskStatuses, List<TaskTracker.Repository.Models.Task>> taskDic = new ();
        TaskRepository repository = new();
        foreach (var status in Enum.GetValues<TaskStatuses>())
        {
            var tasks = await repository.GetTasks(status);
            taskDic.Add(status, tasks);
        }

        // Assert
        foreach (KeyValuePair<TaskStatuses, List<Repository.Models.Task>> pair in taskDic)
        {
            pair.Value.Count.Should().Be(10);
        }
    }

    [Fact]
    public async Task UpdateTest()
    {
        // Arrange
        if (File.Exists(FileName))
        {
            File.Delete(FileName);
        }

        int taskId = 1;
        List<TaskTracker.Repository.Models.Task> taskList = new();
        TaskTracker.Repository.Models.Task task = CreateTask(taskId, "Test task for Update method.");
        taskList.Add(task);
        await SaveTasksAsync(taskList);

        // Act
        TaskRepository repository = new();
        string newDescription = $"Updated task with id={taskId}";
        await repository.Update(taskId, newDescription);

        // Assert
        taskList = await LoadTasksAsync();
        TaskTracker.Repository.Models.Task? updatedTask = taskList.FirstOrDefault(t => t.Id == taskId);

        updatedTask.Should().NotBeNull();
        updatedTask.Description.Should().BeEquivalentTo(newDescription);
        updatedTask.UpdatedAt.Should().NotBeNull();
        updatedTask.UpdatedAt.Should().BeAfter(updatedTask.CreatedAt);
    }

    private TaskTracker.Repository.Models.Task CreateTask(int id, string description)
    {
        TaskTracker.Repository.Models.Task task = new()
        {
            Id = id,
            Description = description,
            CreatedAt = DateTime.Now
        };
        return task;
    }

    private async Task<List<TaskTracker.Repository.Models.Task>> LoadTasksAsync()
    {
        // Read from file
        string jsonString = await File.ReadAllTextAsync(FileName);

        List<TaskTracker.Repository.Models.Task> taskList =
            JsonSerializer.Deserialize<List<TaskTracker.Repository.Models.Task>>(jsonString) ?? [];

        return taskList;
    }

    private async Task SaveTasksAsync(List<TaskTracker.Repository.Models.Task> taskList)
    {
        var contents = JsonSerializer.Serialize(taskList);
        await File.WriteAllTextAsync(FileName, contents);
    }
}