using Microsoft.AspNetCore.Mvc;
using TaskBoard.Api.Services;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentsController(ICommentService commentService) : ControllerBase
{
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var deleted = await commentService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
