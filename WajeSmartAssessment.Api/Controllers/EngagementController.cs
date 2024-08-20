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
    /// <param name="command"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status500InternalServerError)]
    [HttpPost("like")]
    public async Task<IActionResult> LikePost([FromBody] LikePostCommand command)
    {
        var response = await mediator.Send(command);
        return StatusCode(response.StatusCode, response);
    }
}
