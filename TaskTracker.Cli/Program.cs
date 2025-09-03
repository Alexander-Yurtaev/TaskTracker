using TaskTracker.Repository.Json;

namespace TaskTracker.Cli;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Welcome to Task Tracker!");

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
            default:
                Console.WriteLine("You write a wrong command!");
                break;
        }
    }

    private static async Task AddCommand(string[] commandLineParts, TaskRepository repository)
    {
        if (commandLineParts.Length != 2)
        {
            Console.WriteLine("You enter too much or less parameters for Add command.");
            Console.WriteLine("You need enter only Description for task.");
        }
        else
        {
            var description = commandLineParts[1];
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
    }
}