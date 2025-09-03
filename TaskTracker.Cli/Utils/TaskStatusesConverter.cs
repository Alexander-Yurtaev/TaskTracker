using TaskTracker.Repository.Models;

namespace TaskTracker.Cli.Utils;

public static class TaskStatusesConverter
{
    public static TaskStatuses? FromString(string value)
    {
        switch (value)
        {
            case "in-progress":
                return TaskStatuses.InProgress;
            case "done":
                return TaskStatuses.Done;
            case "to-do":
                return TaskStatuses.ToDo;
            default:
                return null;
        }
    }

    public static string ToString(TaskStatuses status)
    {
        switch (status)
        {
            case TaskStatuses.InProgress:
                return "in-progress";
            case TaskStatuses.Done:
                return "done";
            case TaskStatuses.ToDo:
            default:
                return "to-do";
        }
    }
}