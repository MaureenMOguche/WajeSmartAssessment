using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Features.Engagement.Commands;
using WajeSmartAssessment.Application.Middlewares;

namespace WajeSmartAssessment.Api.Controllers;


/// <summary>
/// Engagement controller
/// </summary>
[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class EngagementController(IMediator mediator) : ControllerBase
{

    /// <summary>
    /// Like a post
    /// </summary>
    /// <param name="postId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status500InternalServerError)]
    [HttpPost("like/{postId}")]
    public async Task<IActionResult> LikePost(string postId)
    {
        var response = await mediator.Send(new LikePostCommand(postId));
        return StatusCode(response.StatusCode, response);
    }
}
