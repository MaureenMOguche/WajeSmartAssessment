using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Features.Comments.Commands;
using WajeSmartAssessment.Application.Middlewares;

namespace WajeSmartAssessment.Api.Controllers;

/// <summary>
/// Manages comments
/// </summary>
/// <param name="mediator"></param>
[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
[Authorize("Author")]
public class CommentsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Adds a comment to a blog post
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("add-comment")]
    public async Task<IActionResult> AddComment([FromBody] CreateCommentCommand command)
    {
        var response = await mediator.Send(command);
        return StatusCode(response.StatusCode, response);
    }
}
