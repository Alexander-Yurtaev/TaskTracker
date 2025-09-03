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
        if (command == "exit")
        {
            return;
        }

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
            default:
                Console.WriteLine("You write a wrong command!");
                break;
        }
    }

    private static async Task AddCommand(string[] args, TaskRepository repository)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("You enter too much or less parameters for Add command.");
            Console.WriteLine("You need enter only Description for the task.");
            return;
        }

        var description = args[1];
        try
        {
            var id = await repository.Add(description);
            Console.WriteLine($"Task added successfully (ID: {id})");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static async Task ListCommand(string[] args, TaskRepository repository)
    {
        if (args.Length != 1 && args.Length != 2)
        {
            Console.WriteLine("You enter too much or less parameters for the List command.");
            return;
        }

        try
        {
            var tasks = await repository.GetAllTasks();

            if (args.Length == 2)
            {
                var status = Enum.Parse<TaskStatuses>(args[1]);
                tasks = tasks.Where(t => t.Status == status).ToList();
            }

            Console.WriteLine(new String('-', 44));
            Console.WriteLine($"|{nameof(Repository.Models.Task.Id), -3}|{nameof(Repository.Models.Task.Description), -25}|{nameof(Repository.Models.Task.Status), -12}|");
            Console.WriteLine(new String('-', 44));
            foreach (Repository.Models.Task task in tasks)
            {
                Console.WriteLine($"|{task.Id, -3}|{task.Description, -25}|{task.Status, -12}|");
            }
            Console.WriteLine(new String('-', 44));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static async Task UpdateCommand(string[] args, TaskRepository repository)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("You enter too much or less parameters for Update command.");
            Console.WriteLine("You need enter task id and new Description for the task.");
            return;
        }

        var id = int.Parse(args[1]);
        var description = args[2];
        try
        {
            await repository.Update(id, description);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static async Task DeleteCommand(string[] args, TaskRepository repository)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("You enter too much or less parameters for Delete command.");
            Console.WriteLine("You need enter task id for the task.");
            return;
        }

        var id = int.Parse(args[1]);
        try
        {
            await repository.Delete(id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static async Task UpdateStatusCommand(string[] args, TaskRepository repository, TaskStatuses status)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("You enter too much or less parameters for the command.");
            Console.WriteLine("You need enter task id for the task.");
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
}