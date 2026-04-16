using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Data;
using TaskBoard.Api.DTOs;
using TaskBoard.Api.Models;
using TaskStatus = TaskBoard.Api.Models.TaskStatus;

namespace TaskBoard.Api.Services;

public class TaskService(AppDbContext db) : ITaskService
{
    public async Task<PaginatedResponse<TaskResponse>> GetByProjectAsync(
        int projectId,
        TaskStatus? status,
        TaskPriority? priority,
        string? title,
        string sortBy,
        string sortDir,
        int page,
        int pageSize)
    {
        var exists = await db.Projects.AnyAsync(p => p.Id == projectId);
        if (!exists)
        {
            return new PaginatedResponse<TaskResponse>(Array.Empty<TaskResponse>(), page, pageSize, 0, 0);
        }

        var query = db.Tasks.Where(t => t.ProjectId == projectId).AsQueryable();

        // Only show the statuses required by the assignment UI.
        query = query.Where(t =>
            t.Status == TaskStatus.Todo ||
            t.Status == TaskStatus.InProgress ||
            t.Status == TaskStatus.Review ||
            t.Status == TaskStatus.Done);

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(t => t.Priority == priority.Value);
        }

        if (!string.IsNullOrWhiteSpace(title))
        {
            var titleSearch = title.Trim().ToLower();
            query = query.Where(t => t.Title.ToLower().Contains(titleSearch));
        }

        var sort = sortBy.Trim().ToLower();
        var desc = sortDir.Trim().ToLower() != "asc";

        query = (sort, desc) switch
        {
            ("duedate", true) => query.OrderByDescending(t => t.DueDate),
            ("duedate", false) => query.OrderBy(t => t.DueDate),
            ("priority", true) => query.OrderByDescending(t => t.Priority),
            ("priority", false) => query.OrderBy(t => t.Priority),
            ("createdat", false) => query.OrderBy(t => t.CreatedAt),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };

        var totalCount = await query.CountAsync();
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TaskResponse(
                t.Id,
                t.ProjectId,
                t.Title,
                t.Description,
                t.Priority,
                t.Status,
                t.DueDate,
                t.CreatedAt,
                t.UpdatedAt))
            .ToListAsync();

        return new PaginatedResponse<TaskResponse>(items, page, pageSize, totalCount, totalPages);
    }

    public async Task<TaskResponse?> CreateAsync(int projectId, TaskCreateRequest request)
    {
        var projectExists = await db.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists)
        {
            return null;
        }

        if (request.DueDate.HasValue && request.DueDate.Value.Date < DateTime.Now.Date)
        {
            throw new ArgumentException("Due date must be today or a future date.");
        }

        var task = new TaskItem
        {
            ProjectId = projectId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Priority = request.Priority,
            Status = request.Status,
            DueDate = request.DueDate
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        return new TaskResponse(task.Id, task.ProjectId, task.Title, task.Description, task.Priority, task.Status, task.DueDate, task.CreatedAt, task.UpdatedAt);
    }

    public async Task<TaskWithCommentsResponse?> GetByIdAsync(int id)
    {
        var task = await db.Tasks
            .Include(t => t.Comments)
            .Include(t => t.Histories)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null)
        {
            return null;
        }

        var comments = task.Comments
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CommentResponse(c.Id, c.TaskId, c.Author, c.Body, c.CreatedAt))
            .ToList();

        var history = task.Histories
            .OrderByDescending(h => h.ChangedAt)
            .Select(h => new TaskHistoryResponse(h.Id, h.TaskId, h.OldStatus, h.NewStatus, h.ChangedAt))
            .ToList();

        return new TaskWithCommentsResponse(
            task.Id, task.ProjectId, task.Title, task.Description, task.Priority, task.Status,
            task.DueDate, task.CreatedAt, task.UpdatedAt, comments, history);
    }

    public async Task<TaskResponse?> UpdateAsync(int id, TaskUpdateRequest request)
    {
        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (task is null)
        {
            return null;
        }

        if (request.DueDate.HasValue && request.DueDate.Value.Date < DateTime.Now.Date)
        {
            throw new ArgumentException("Due date must be today or a future date.");
        }

        var oldStatus = task.Status;

        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim();
        task.Priority = request.Priority;
        task.Status = request.Status;
        task.DueDate = request.DueDate;

        if (oldStatus != request.Status)
        {
            db.TaskHistories.Add(new TaskHistory
            {
                TaskId = task.Id,
                OldStatus = oldStatus,
                NewStatus = request.Status
            });
        }

        await db.SaveChangesAsync();

        return new TaskResponse(task.Id, task.ProjectId, task.Title, task.Description, task.Priority, task.Status, task.DueDate, task.CreatedAt, task.UpdatedAt);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (task is null)
        {
            return false;
        }

        db.Tasks.Remove(task);
        await db.SaveChangesAsync();
        return true;
    }
}
