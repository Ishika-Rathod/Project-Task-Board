namespace TaskBoard.Api.Models;

public enum TaskPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum TaskStatus
{
    Todo = 1,
    InProgress = 2,
    Review = 3,
    // Keep underlying numeric value compatible with earlier versions where Done was 7.
    Done = 7
}
