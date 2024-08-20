using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Dtos;
using WajeSmartAssessment.Application.Features.Blogs.Commands;
using WajeSmartAssessment.Application.Features.Blogs.Queries;
using WajeSmartAssessment.Application.Middlewares;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Api.Controllers;

/// <summary>
/// Blogs management
/// </summary>
[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class BlogsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets all blogs
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [HttpGet("all-blogs")]
    public async Task<IActionResult> GetAllBlogs([FromQuery]BaseQueryParams queryParams)
    {
        var response = await mediator.Send(new GetAllBlogsQuery(queryParams));
        return StatusCode(response.StatusCode, response);
    }



    /// <summary>
    /// Gets a blog by it's Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [HttpGet("blogs/{id}")]
    public async Task<IActionResult> GetBlogById(string id)
    {
        var response = await mediator.Send(new GetBlogByIdQuery(id));
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Adds a new blog
    /// </summary>
    /// <param name="blog"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("add-blog")]
    [Authorize("Admin")]
    public async Task<IActionResult> CreateBlog([FromBody] CreateBlogCommand blog)
    {
        var response = await mediator.Send(blog);
        return StatusCode(response.StatusCode, response);
    }
}
