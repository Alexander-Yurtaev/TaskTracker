using FluentAssertions;
using System.Text.Json;
using TaskTracker.Repository.Json;
using TaskTracker.Repository.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskTracker.Tests.Json;

public class TaskRepositoryTests
{
    private const string FileName = "TaskTracker.json";

    private readonly TaskRepository _repository;

    public TaskRepositoryTests()
    {
        if (File.Exists(FileName))
        {
            File.Delete(FileName);
        }

        _repository = new TaskRepository();
    }

    [Fact]
    public async Task Should_Successfully_Add_New_Task_And_Create_File()
    {
        // Arrange
        string taskDescription = "New Test Task";

        // Act
        var id = await _repository.Add(taskDescription);

        // Assert
        id.Should().BeGreaterThan(0, "Id was not initial");
        File.Exists(FileName).Should().BeTrue("File was not created.");

        string jsonString = await File.ReadAllTextAsync(FileName);
        List<TaskTracker.Repository.Models.Task>? taskList = JsonSerializer.Deserialize<List<TaskTracker.Repository.Models.Task>>(jsonString);

        taskList.Should().NotBeNull();
        taskList.Count.Should().Be(1);

        var task = taskList.First();

        task.Id.Should().Be(1);
        task.Description.Should().BeEquivalentTo(taskDescription);
        task.Status.Should().Be(TaskStatuses.ToDo);
        task.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public async Task Should_Return_All_Tasks_When_Multiple_Tasks_Exist()
    {
        // Arrange
        await SeedTasks(10);

        // Act
        var tasks = await _repository.GetAllTasks();

        // Assert
        tasks.Should().NotBeNullOrEmpty();
        tasks.Count.Should().Be(10);

        foreach (var task in tasks)
        {
            task.Description.Should().StartWith("Test task #");
        }
    }

    [Fact]
    public async Task Should_Correctly_Filter_Tasks_By_Status()
    {
        // Arrange
        await SeedTasks(Enum.GetValues<TaskStatuses>().Length * 10);
        List<TaskTracker.Repository.Models.Task> taskList = await LoadTasksAsync();
        foreach (var status in Enum.GetValues<TaskStatuses>())
        {
            for (int i = 1; i <= 10; i++)
            {
                int id = i + 10*(int)status;
                var task = taskList.Single(t => t.Id == id);
                task.Status = status;
            }
        }
        await SaveTasksAsync(taskList);

        // Act
        Dictionary<TaskStatuses, List<TaskTracker.Repository.Models.Task>> taskDic = new ();
        foreach (var status in Enum.GetValues<TaskStatuses>())
        {
            var tasks = await _repository.GetTasks(status);
            taskDic.Add(status, tasks);
        }

        // Assert
        foreach (KeyValuePair<TaskStatuses, List<Repository.Models.Task>> pair in taskDic)
        {
            pair.Value.Count.Should().Be(10);
        }
    }

    [Fact]
    public async Task Should_Successfully_Update_Task_Description_And_Timestamp()
    {
        // Arrange
        int taskId = 1;
        List<TaskTracker.Repository.Models.Task> taskList = new();
        TaskTracker.Repository.Models.Task task = CreateTask(taskId, "Test task for Update method.");
        taskList.Add(task);
        await SaveTasksAsync(taskList);

        // Act
        string newDescription = $"Updated task with id={taskId}";
        await _repository.Update(taskId, newDescription);

        // Assert
        taskList = await LoadTasksAsync();
        TaskTracker.Repository.Models.Task? updatedTask = taskList.FirstOrDefault(t => t.Id == taskId);

        updatedTask.Should().NotBeNull();
        updatedTask.Description.Should().BeEquivalentTo(newDescription);
        updatedTask.Status.Should().Be(task.Status);
        updatedTask.CreatedAt.Should().Be(task.CreatedAt);
        updatedTask.UpdatedAt.Should().NotBeNull();
        updatedTask.UpdatedAt.Should().BeAfter(updatedTask.CreatedAt);
    }

    [Theory]
    [InlineData(TaskStatuses.InProgress, TaskStatuses.ToDo)]
    [InlineData(TaskStatuses.ToDo, TaskStatuses.Done)]
    public async Task Should_Successfully_Change_Task_Status_When_Valid_Status_Provided(TaskStatuses oldStatus, TaskStatuses newStatus)
    {
        // Arrange
        int taskId = 1;
        List<TaskTracker.Repository.Models.Task> taskList = new();
        TaskTracker.Repository.Models.Task task = CreateTask(taskId, "Test task for Update method.");
        task.Status = oldStatus;
        taskList.Add(task);
        await SaveTasksAsync(taskList);

        // Act
        await _repository.UpdateStatus(taskId, newStatus);

        // Assert
        taskList = await LoadTasksAsync();
        TaskTracker.Repository.Models.Task? updatedTask = taskList.FirstOrDefault(t => t.Id == taskId);

        updatedTask.Should().NotBeNull();
        updatedTask.Status.Should().Be(newStatus);
        updatedTask.UpdatedAt.Should().NotBeNull();
        updatedTask.UpdatedAt.Should().BeAfter(updatedTask.CreatedAt);
    }

    [Fact]
    public async Task Should_Successfully_Delete_Existing_Task_ById()
    {
        // Arrange
        int deletedId = 5;

        List<TaskTracker.Repository.Models.Task> taskList = new();
        await SeedTasks(10);

        // Act
        await _repository.Delete(deletedId);

        // Assert
        taskList = await LoadTasksAsync();

        taskList.Count.Should().Be(9);
        taskList.Any(t => t.Id == deletedId).Should().BeFalse();
    }

    [Fact]
    public async Task Should_Not_Throw_Exception_When_Deleting_NonExistent_Task()
    {
        // Arrange
        int deletedId = 55;

        List<TaskTracker.Repository.Models.Task> taskList = new();
        await SeedTasks(10);

        // Act
        Func<Task> act = async () => await _repository.Delete(deletedId);

        // Assert
        taskList.SingleOrDefault(t => t.Id == deletedId).Should().BeNull();
        await act.Should().NotThrowAsync<Exception>();
    }

    [Fact]
    public async Task Should_Successfully_Delete_Last_Task()
    {
        // Arrange
        int deletedId = 1;

        List<TaskTracker.Repository.Models.Task> taskList = new();
        TaskTracker.Repository.Models.Task task = CreateTask(deletedId, $"Test task for Delete method #{deletedId}.");
        taskList.Add(task);
        await SaveTasksAsync(taskList);

        // Act
        await _repository.Delete(deletedId);

        // Assert
        taskList = await LoadTasksAsync();

        taskList.Count.Should().Be(0);
    }

    #region Private Methods

    private async Task SeedTasks(int count)
    {
        List<TaskTracker.Repository.Models.Task> taskList = new();
        for (int i = 1; i <= count; i++)
        {
            var task = CreateTask(i, $"Test task #{i}");
            taskList.Add(task);
        }
        await SaveTasksAsync(taskList);
    }

    private TaskTracker.Repository.Models.Task CreateTask(int id, string description)
    {
        TaskTracker.Repository.Models.Task task = new(id, description);
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

    #endregion Private Methods
}