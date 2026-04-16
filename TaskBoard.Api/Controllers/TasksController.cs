using Microsoft.AspNetCore.Mvc;
using TaskBoard.Api.DTOs;
using TaskBoard.Api.Services;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController(ITaskService taskService, ICommentService commentService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskWithCommentsResponse>> GetTaskById(int id)
    {
        var task = await taskService.GetByIdAsync(id);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskResponse>> UpdateTask(int id, [FromBody] TaskUpdateRequest request)
    {
        try
        {
            var task = await taskService.UpdateAsync(id, request);
            return task is null ? NotFound() : Ok(task);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { errors = new Dictionary<string, string[]> { ["dueDate"] = new[] { ex.Message } } });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var deleted = await taskService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{taskId:int}/comments")]
    public async Task<ActionResult<IReadOnlyList<CommentResponse>>> GetCommentsByTask(int taskId)
    {
        var comments = await commentService.GetByTaskAsync(taskId);
        return comments is null ? NotFound() : Ok(comments);
    }

    [HttpPost("{taskId:int}/comments")]
    public async Task<ActionResult<CommentResponse>> CreateComment(int taskId, [FromBody] CommentCreateRequest request)
    {
        var comment = await commentService.CreateAsync(taskId, request);
        return comment is null
            ? NotFound(new { message = "Task not found." })
            : CreatedAtAction(nameof(GetCommentsByTask), new { taskId }, comment);
    }
}
