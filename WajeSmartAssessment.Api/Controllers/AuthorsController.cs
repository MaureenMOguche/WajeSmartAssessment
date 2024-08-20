using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Dtos;
using WajeSmartAssessment.Application.Features.Authors.Commands;
using WajeSmartAssessment.Application.Features.Authors.Queries;
using WajeSmartAssessment.Application.Middlewares;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Api.Controllers;


/// <summary>
/// Manages all the operations related to authors
/// </summary>
/// <param name="mediator"></param>
[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
[Authorize("Admin")]
public class AuthorsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get all authors
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [HttpGet("authors")]
    public async Task<IActionResult> GetAuthors([FromQuery]BaseQueryParams queryParams)
    {
        var response = await mediator.Send(new GetAllAuthorsQuery(queryParams));
        return StatusCode(response.StatusCode, response);
    }


    /// <summary>
    /// Get an author by Id
    /// </summary>
    /// <param name="authorId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("authors/{authorId}")]
    public async Task<IActionResult> GetAuthorById(string authorId)
    {
        var response = await mediator.Send(new GetAuthorByIdQuery(authorId));
        return StatusCode(response.StatusCode, response);
    }



    /// <summary>
    /// Disables or enables an author from making posts.
    /// </summary>
    /// <param name="authorId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPut("disable-enable-author/{authorId}")]
    public async Task<IActionResult> DisableEnableAuthor(string authorId)
    {
        var response = await mediator.Send(new DisableEnableAuthorCommand(authorId));
        return StatusCode(response.StatusCode, response);
    }
}
