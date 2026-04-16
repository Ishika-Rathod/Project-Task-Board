using TaskBoard.Api.DTOs;
using TaskBoard.Api.Models;
using TaskStatus = TaskBoard.Api.Models.TaskStatus;

namespace TaskBoard.Api.Services;

public interface IProjectService
{
    Task<IReadOnlyList<ProjectSummaryResponse>> GetAllAsync();
    Task<ProjectDetailResponse?> GetByIdAsync(int id);
    Task<(bool IsDuplicate, ProjectSummaryResponse? Project)> CreateAsync(ProjectCreateRequest request);
    Task<(bool NotFound, bool IsDuplicate, ProjectSummaryResponse? Project)> UpdateAsync(int id, ProjectUpdateRequest request);
    Task<bool> DeleteAsync(int id);
}

public interface ITaskService
{
    Task<PaginatedResponse<TaskResponse>> GetByProjectAsync(
        int projectId,
        TaskStatus? status,
        TaskPriority? priority,
        string? title,
        string sortBy,
        string sortDir,
        int page,
        int pageSize);

    Task<TaskResponse?> CreateAsync(int projectId, TaskCreateRequest request);
    Task<TaskWithCommentsResponse?> GetByIdAsync(int id);
    Task<TaskResponse?> UpdateAsync(int id, TaskUpdateRequest request);
    Task<bool> DeleteAsync(int id);
}

public interface ICommentService
{
    Task<IReadOnlyList<CommentResponse>?> GetByTaskAsync(int taskId);
    Task<CommentResponse?> CreateAsync(int taskId, CommentCreateRequest request);
    Task<bool> DeleteAsync(int id);
}

public interface IDashboardService
{
    Task<DashboardResponse> GetSummaryAsync();
}
