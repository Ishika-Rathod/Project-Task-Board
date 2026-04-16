using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Data;
using TaskBoard.Api.DTOs;
using TaskBoard.Api.Models;

namespace TaskBoard.Api.Services;

public class CommentService(AppDbContext db) : ICommentService
{
    public async Task<IReadOnlyList<CommentResponse>?> GetByTaskAsync(int taskId)
    {
        var taskExists = await db.Tasks.AnyAsync(t => t.Id == taskId);
        if (!taskExists)
        {
            return null;
        }

        var comments = await db.Comments
            .Where(c => c.TaskId == taskId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CommentResponse(c.Id, c.TaskId, c.Author, c.Body, c.CreatedAt))
            .ToListAsync();

        return comments;
    }

    public async Task<CommentResponse?> CreateAsync(int taskId, CommentCreateRequest request)
    {
        var taskExists = await db.Tasks.AnyAsync(t => t.Id == taskId);
        if (!taskExists)
        {
            return null;
        }

        var comment = new Comment
        {
            TaskId = taskId,
            Author = request.Author.Trim(),
            Body = request.Body.Trim()
        };

        db.Comments.Add(comment);
        await db.SaveChangesAsync();

        return new CommentResponse(comment.Id, comment.TaskId, comment.Author, comment.Body, comment.CreatedAt);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var comment = await db.Comments.FirstOrDefaultAsync(c => c.Id == id);
        if (comment is null)
        {
            return false;
        }

        db.Comments.Remove(comment);
        await db.SaveChangesAsync();
        return true;
    }
}
