using TaskBoard.Api.Models;
using TaskStatus = TaskBoard.Api.Models.TaskStatus;

namespace TaskBoard.Api.Data;

public static class SeedData
{
    public static void EnsureSeeded(AppDbContext db)
    {
        if (db.Projects.Any())
        {
            return;
        }

        var websiteProject = new Project
        {
            Name = "Website Revamp",
            Description = "Update marketing site UX and performance."
        };

        var mobileProject = new Project
        {
            Name = "Mobile App Launch",
            Description = "Prepare v1 Android and iOS release tasks."
        };

        db.Projects.AddRange(websiteProject, mobileProject);
        db.SaveChanges();

        var tasks = new List<TaskItem>
        {
            new()
            {
                ProjectId = websiteProject.Id,
                Title = "Design homepage hero",
                Description = "Create mockups and finalize CTA copy.",
                Priority = TaskPriority.High,
                Status = TaskStatus.InProgress,
                DueDate = DateTime.UtcNow.Date.AddDays(2)
            },
            new()
            {
                ProjectId = websiteProject.Id,
                Title = "Fix core web vitals",
                Description = "Improve LCP and CLS scores.",
                Priority = TaskPriority.Critical,
                Status = TaskStatus.Todo,
                DueDate = DateTime.UtcNow.Date.AddDays(1)
            },
            new()
            {
                ProjectId = mobileProject.Id,
                Title = "Prepare app store screenshots",
                Description = "Collect screens for all main flows.",
                Priority = TaskPriority.Medium,
                Status = TaskStatus.Review,
                DueDate = DateTime.UtcNow.Date.AddDays(5)
            }
        };

        db.Tasks.AddRange(tasks);
        db.SaveChanges();

        db.Comments.AddRange(
            new Comment
            {
                TaskId = tasks[0].Id,
                Author = "Alex",
                Body = "Please align this with brand guidelines."
            },
            new Comment
            {
                TaskId = tasks[0].Id,
                Author = "Riya",
                Body = "CTA color updated in latest mockup."
            },
            new Comment
            {
                TaskId = tasks[1].Id,
                Author = "Sam",
                Body = "Need Lighthouse baseline before optimization."
            }
        );

        db.SaveChanges();
    }
}
