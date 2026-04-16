using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Data;
using TaskBoard.Api.DTOs;
using TaskBoard.Api.Models;
using TaskStatus = TaskBoard.Api.Models.TaskStatus;

namespace TaskBoard.Api.Services;

public class DashboardService(AppDbContext db) : IDashboardService
{
    public async Task<DashboardResponse> GetSummaryAsync()
    {
        var totalProjects = await db.Projects.CountAsync();
        var totalTasks = await db.Tasks.CountAsync();
        var now = DateTime.UtcNow;
        var in7Days = now.AddDays(7);

        var allowedTasks = db.Tasks.Where(t =>
            t.Status == TaskStatus.Todo ||
            t.Status == TaskStatus.InProgress ||
            t.Status == TaskStatus.Review ||
            t.Status == TaskStatus.Done);

        var tasksByStatus = await allowedTasks
            .GroupBy(t => t.Status)
            .ToDictionaryAsync(g => g.Key.ToString(), g => g.Count());

        foreach (var status in Enum.GetValues<TaskStatus>())
        {
            tasksByStatus.TryAdd(status.ToString(), 0);
        }

        var overdueCount = await allowedTasks
            .CountAsync(t =>
                t.DueDate.HasValue &&
                t.DueDate.Value < now &&
                t.Status != TaskStatus.Done);

        var dueWithin7Days = await allowedTasks
            .CountAsync(t =>
                t.DueDate.HasValue &&
                t.DueDate.Value >= now &&
                t.DueDate.Value <= in7Days &&
                t.Status != TaskStatus.Done);

        return new DashboardResponse(totalProjects, totalTasks, tasksByStatus, overdueCount, dueWithin7Days);
    }
}
