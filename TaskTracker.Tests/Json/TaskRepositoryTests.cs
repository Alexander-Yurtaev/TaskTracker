using System.Text.Json;
using FluentAssertions;
using TaskTracker.Repository.Json;

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

        TaskRepository repository = new();

        // Act
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

        TaskRepository repository = new();
        for (int i = 0; i < 10; i++)
        {
            await repository.Add($"Test task for GetAllTasks method #{i}.");
        }
        
        // Act
        var tasks = await repository.GetAllTasks();

        // Assert
        tasks.Should().NotBeNullOrEmpty();
        tasks.Count.Should().Be(10);

        foreach (var task in tasks)
        {
            task.Description.Should().StartWith("Test task for GetAllTasks method #");
        }
    }
}