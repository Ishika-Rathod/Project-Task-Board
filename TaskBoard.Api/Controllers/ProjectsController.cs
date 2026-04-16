using Microsoft.AspNetCore.Mvc;
using TaskBoard.Api.DTOs;
using TaskBoard.Api.Services;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController(IProjectService projectService, ITaskService taskService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectSummaryResponse>>> GetProjects()
    {
        var projects = await projectService.GetAllAsync();
        return Ok(projects);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDetailResponse>> GetProjectById(int id)
    {
        var project = await projectService.GetByIdAsync(id);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectSummaryResponse>> CreateProject([FromBody] ProjectCreateRequest request)
    {
        var result = await projectService.CreateAsync(request);
        if (result.IsDuplicate)
        {
            return Conflict(new { message = "A project with this name already exists." });
        }

        return CreatedAtAction(nameof(GetProjectById), new { id = result.Project!.Id }, result.Project);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProjectSummaryResponse>> UpdateProject(int id, [FromBody] ProjectUpdateRequest request)
    {
        var result = await projectService.UpdateAsync(id, request);
        if (result.NotFound)
        {
            return NotFound();
        }

        if (result.IsDuplicate)
        {
            return Conflict(new { message = "A project with this name already exists." });
        }

        return Ok(result.Project);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var deleted = await projectService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{projectId:int}/tasks")]
    public async Task<ActionResult<PaginatedResponse<TaskResponse>>> GetTasksByProject(
        int projectId,
        [FromQuery] Models.TaskStatus? status,
        [FromQuery] Models.TaskPriority? priority,
        [FromQuery] string? title,
        [FromQuery] string sortBy = "createdAt",
        [FromQuery] string sortDir = "desc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var data = await taskService.GetByProjectAsync(projectId, status, priority, title, sortBy, sortDir, page, pageSize);
        return Ok(data);
    }

    [HttpPost("{projectId:int}/tasks")]
    public async Task<ActionResult<TaskResponse>> CreateTask(int projectId, [FromBody] TaskCreateRequest request)
    {
        try
        {
            var task = await taskService.CreateAsync(projectId, request);
            if (task is null)
            {
                return NotFound(new { message = "Project not found." });
            }

            return CreatedAtAction(nameof(TasksController.GetTaskById), "Tasks", new { id = task.Id }, task);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { errors = new Dictionary<string, string[]> { ["dueDate"] = new[] { ex.Message } } });
        }
    }
}
