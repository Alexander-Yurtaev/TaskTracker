using System.Text.Json;
using FluentAssertions;
using TaskTracker.Repository.Json;

namespace TaskTracker.Tests.Json;

public class TaskRepositoryTests
{
    [Fact]
    public async Task AddTest()
    {
        string fileName = "TaskTracker.json";

        // Arrange
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }

        TaskRepository repository = new();

        // Act
        var id = await repository.Add("New Test Task");

        // Assert
        id.Should().BeGreaterThan(0, "Id was not initial");
        File.Exists(fileName).Should().BeTrue("File was not created.");

        string jsonString = await File.ReadAllTextAsync(fileName);
        List<TaskTracker.Repository.Models.Task>? taskList = JsonSerializer.Deserialize<List<TaskTracker.Repository.Models.Task>>(jsonString);

        taskList.Should().NotBeNull();
        taskList.Count.Should().Be(1);
    }
}