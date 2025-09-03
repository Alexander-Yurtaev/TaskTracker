using TaskTracker.Cli.Utils;
using TaskTracker.Repository.Json;
using TaskTracker.Repository.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskTracker.Cli;

public static class Program
{
    public static async Task Main(string[] args)
    {
        TaskRepository repository = new TaskRepository();

        if (args.Length == 0)
        {
            Console.WriteLine("You must write a command.");
            return;
        }

        var command = args[0];
        switch (command)
        {
            case "add":
                await AddCommand(args, repository);
                break;
            case "list":
                await ListCommand(args, repository);
                break;
            case "update":
                await UpdateCommand(args, repository);
                break;
            case "delete":
                await DeleteCommand(args, repository);
                break;
            case "mark-in-progress":
                await UpdateStatusCommand(args, repository, TaskStatuses.InProgress);
                break;
            case "mark-done":
                await UpdateStatusCommand(args, repository, TaskStatuses.Done);
                break;
            case "help":
                PrintCommandList();
                break;
            default:
                Console.WriteLine("Error: Unknown command!");
                PrintCommandList();
                break;
        }
    }

    private static async Task AddCommand(string[] args, TaskRepository repository)
    {
        if (!ValidateParameters(args, [0], "Add"))
        {
            Console.WriteLine("Usage: add <description>");
            return;
        }

        var description = args[1];
        try
        {
            var id = await repository.Add(description);
            Console.WriteLine($"Task added successfully (ID: {id})");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error: Invalid description - {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding task: {ex.Message}");
        }
    }

    private static async Task ListCommand(string[] args, TaskRepository repository)
    {
        if (!ValidateParameters(args, [1, 2], "List"))
        {
            Console.WriteLine("Usage: list [status]");
            return;
        }

        try
        {
            var tasks = await repository.GetAllTasks();

            if (args.Length == 2)
            {
                TaskStatuses? status = TaskStatusesConverter.FromString(args[1]);
                if (status is not null)
                {
                    tasks = tasks.Where(t => t.Status == status).ToList();
                }
                else
                {
                    Console.WriteLine("Error: Unknown task status.");
                    return;
                }
            }

            PrintTableHeader();

            foreach (Repository.Models.Task task in tasks)
            {
                PrintTask(task);
            }

            PrintFooter();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error listing tasks: {ex.Message}");
        }
    }

    private static async Task UpdateCommand(string[] args, TaskRepository repository)
    {
        if (!ValidateParameters(args, [3], "Update"))
        {
            Console.WriteLine("Usage: update <id> <description>");
            return;
        }

        var id = int.Parse(args[1]);
        var description = args[2];
        try
        {
            await repository.Update(id, description);
        }
        catch (KeyNotFoundException)
        {
            Console.WriteLine($"Error: Task with ID {args[1]} not found");
        }
        catch (FormatException)
        {
            Console.WriteLine("Error: Invalid task ID format");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating task: {ex.Message}");
        }
    }

    private static async Task DeleteCommand(string[] args, TaskRepository repository)
    {
        if (!ValidateParameters(args, [2], "Delete"))
        {
            Console.WriteLine("Usage: delete <id>");
            return;
        }

        var id = int.Parse(args[1]);
        try
        {
            await repository.Delete(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting task: {ex.Message}");
        }
    }

    private static async Task UpdateStatusCommand(string[] args, TaskRepository repository, TaskStatuses status)
    {
        if (!ValidateParameters(args, [2], "UpdateStatus"))
        {
            return;
        }

        var id = int.Parse(args[1]);
        try
        {
            await repository.UpdateStatus(id, status);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static void PrintCommandList()
    {
        Console.WriteLine("Available commands:");
        Console.WriteLine("  add <description>");
        Console.WriteLine("  list [status]");
        Console.WriteLine("  update <id> <description>");
        Console.WriteLine("  delete <id>");
        Console.WriteLine("  mark-in-progress <id>");
        Console.WriteLine("  mark-done <id>");
        Console.WriteLine("  help");
    }

    private static bool ValidateParameters(string[] args, int[] expectedLengths, string commandName)
    {
        if (!expectedLengths.Contains(args.Length))
        {
            Console.WriteLine($"Error: Invalid number of parameters for '{commandName}' command");
            Console.WriteLine($"Usage: {commandName} <parameters>");
            return false;
        }
        return true;
    }


    private static void PrintTableHeader()
    {
        Console.WriteLine(new String('-', 44));
        Console.WriteLine($"|{nameof(Repository.Models.Task.Id),-3}|{nameof(Repository.Models.Task.Description),-25}|{nameof(Repository.Models.Task.Status),-12}|");
        Console.WriteLine(new String('-', 44));
    }

    private static void PrintTask(TaskTracker.Repository.Models.Task task)
    {
        Console.WriteLine($"|{task.Id,-3}|{task.Description,-25}|{TaskStatusesConverter.ToString(task.Status),-12}|");
    }

    private static void PrintFooter()
    {
        Console.WriteLine(new String('-', 44));
    }
}