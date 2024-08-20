using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Features.Comments.Commands;

namespace WajeSmartAssessment.Api.Controllers;

/// <summary>
/// Manages comments
/// </summary>
/// <param name="mediator"></param>
[Route("api/[controller]")]
[ApiController]
public class CommentsController(IMediator mediator) : ControllerBase
{
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
