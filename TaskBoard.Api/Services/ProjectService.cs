using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Data;
using TaskBoard.Api.DTOs;
using TaskBoard.Api.Models;
using TaskStatus = TaskBoard.Api.Models.TaskStatus;

namespace TaskBoard.Api.Services;

public class ProjectService(AppDbContext db) : IProjectService
{
    public async Task<IReadOnlyList<ProjectSummaryResponse>> GetAllAsync()
    {
        var projects = await db.Projects
            .Include(p => p.Tasks)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return projects.Select(MapProjectSummary).ToList();
    }

    public async Task<ProjectDetailResponse?> GetByIdAsync(int id)
    {
        var project = await db.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project is null)
        {
            return null;
        }

        var tasks = project.Tasks
            .OrderByDescending(t => t.CreatedAt)
            .Select(MapTaskResponse)
            .ToList();

        return new ProjectDetailResponse(
            project.Id,
            project.Name,
            project.Description,
            project.CreatedAt,
            tasks
        );
    }

    public async Task<(bool IsDuplicate, ProjectSummaryResponse? Project)> CreateAsync(ProjectCreateRequest request)
    {
        var existing = await db.Projects
            .AnyAsync(p => p.Name.ToLower() == request.Name.Trim().ToLower());

        if (existing)
        {
            return (true, null);
        }

        var project = new Project
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim()
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync();

        return (false, MapProjectSummary(project));
    }

    public async Task<(bool NotFound, bool IsDuplicate, ProjectSummaryResponse? Project)> UpdateAsync(int id, ProjectUpdateRequest request)
    {
        var project = await db.Projects.Include(p => p.Tasks).FirstOrDefaultAsync(p => p.Id == id);
        if (project is null)
        {
            return (true, false, null);
        }

        var duplicate = await db.Projects
            .AnyAsync(p => p.Id != id && p.Name.ToLower() == request.Name.Trim().ToLower());
        if (duplicate)
        {
            return (false, true, null);
        }

        project.Name = request.Name.Trim();
        project.Description = request.Description?.Trim();
        await db.SaveChangesAsync();

        return (false, false, MapProjectSummary(project));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project is null)
        {
            return false;
        }

        db.Projects.Remove(project);
        await db.SaveChangesAsync();
        return true;
    }

    private static ProjectSummaryResponse MapProjectSummary(Project project)
    {
        Dictionary<string, int> taskCounts = Enum.GetValues<TaskStatus>()
            .ToDictionary(
                status => status.ToString(),
                status => project.Tasks.Count(t => t.Status == status)
            );

        return new ProjectSummaryResponse(
            project.Id,
            project.Name,
            project.Description,
            project.CreatedAt,
            taskCounts
        );
    }

    private static TaskResponse MapTaskResponse(TaskItem t) =>
        new(
            t.Id,
            t.ProjectId,
            t.Title,
            t.Description,
            t.Priority,
            t.Status,
            t.DueDate,
            t.CreatedAt,
            t.UpdatedAt
        );
}
