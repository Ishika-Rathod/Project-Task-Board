namespace TaskBoard.Api.Models;

public class TaskHistory
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public TaskStatus OldStatus { get; set; }
    public TaskStatus NewStatus { get; set; }
    public DateTime ChangedAt { get; set; }

    public TaskItem? Task { get; set; }
}
