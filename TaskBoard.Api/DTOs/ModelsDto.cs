using System.ComponentModel.DataAnnotations;
using TaskBoard.Api.Models;
using TaskStatus = TaskBoard.Api.Models.TaskStatus;

namespace TaskBoard.Api.DTOs;

public class ProjectCreateRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }
}

public class ProjectUpdateRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }
}

public record ProjectSummaryResponse(
    int Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    Dictionary<string, int> TaskCounts
);

public record ProjectDetailResponse(
    int Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    List<TaskResponse> Tasks
);

public class TaskCreateRequest
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public TaskPriority Priority { get; set; }

    [Required]
    public TaskStatus Status { get; set; }

    public DateTime? DueDate { get; set; }
}

public class TaskUpdateRequest : TaskCreateRequest;

public record TaskResponse(
    int Id,
    int ProjectId,
    string Title,
    string? Description,
    TaskPriority Priority,
    TaskStatus Status,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record TaskWithCommentsResponse(
    int Id,
    int ProjectId,
    string Title,
    string? Description,
    TaskPriority Priority,
    TaskStatus Status,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<CommentResponse> Comments,
    List<TaskHistoryResponse> History
);

public record TaskHistoryResponse(
    int Id,
    int TaskId,
    TaskStatus OldStatus,
    TaskStatus NewStatus,
    DateTime ChangedAt
);

public record CommentResponse(
    int Id,
    int TaskId,
    string Author,
    string Body,
    DateTime CreatedAt
);

public class CommentCreateRequest
{
    [Required]
    [MaxLength(50)]
    public string Author { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Body { get; set; } = string.Empty;
}

public record DashboardResponse(
    int TotalProjects,
    int TotalTasks,
    Dictionary<string, int> TasksByStatus,
    int OverdueCount,
    int DueWithin7Days
);

public record PaginatedResponse<T>(
    IReadOnlyList<T> Data,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);
