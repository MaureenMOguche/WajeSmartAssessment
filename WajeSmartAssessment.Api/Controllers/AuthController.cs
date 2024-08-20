using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Features.Auth.Commands;

namespace WajeSmartAssessment.Api.Controllers;

/// <summary>
/// Authentication controller
/// </summary>
[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Allows an author to register
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [HttpPost("register-author")]
    public async Task<IActionResult> Register([FromForm]RegisterAuthorCommand command)
    {
        var response = await mediator.Send(command);
        return StatusCode(response.StatusCode, response);
    }


    /// <summary>
    /// Logs in a user
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var response = await mediator.Send(command);
        return StatusCode(response.StatusCode, response);
    }
}
