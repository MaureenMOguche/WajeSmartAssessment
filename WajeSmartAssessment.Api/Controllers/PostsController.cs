using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Dtos;
using WajeSmartAssessment.Application.Features.Posts.Commands;
using WajeSmartAssessment.Application.Features.Posts.Queries;
using WajeSmartAssessment.Application.Middlewares;

namespace WajeSmartAssessment.Api.Controllers;

/// <summary>
/// Posts management
/// </summary>
[Authorize]
[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
public class PostsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets all posts
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [HttpGet("posts")]
    public async Task<IActionResult> GetAllPosts([FromQuery] BaseQueryParams queryParams)
    {
        var response = await mediator.Send(new GetAllPostsQuery(queryParams));
        return StatusCode(response.StatusCode, response);
    }


    /// <summary>
    /// Gets a post by it's id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [HttpGet("posts/{id}")]
    public async Task<IActionResult> GetPostById(string id)
    {
        var response = await mediator.Send(new GetPostByIdQuery(id));
        return StatusCode(response.StatusCode, response);
    }


    /// <summary>
    /// Adds a new post to a blog
    /// </summary>
    /// <param name="post"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("add-post")]
    public async Task<IActionResult> CreatePost([FromForm]CreatePostCommand post)
    {
        var response = await mediator.Send(post);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Gets posts by an author
    /// </summary>
    /// <param name="authorId"></param>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [HttpGet("author-posts/{authorId}")]
    public async Task<IActionResult> GetAuthorPosts(string authorId, [FromQuery] BaseQueryParams queryParams)
    {
        var response = await mediator.Send(new GetPostsByAuthorQuery(authorId, queryParams));
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Gets a blog's posts
    /// </summary>
    /// <param name="queryParams"></param>
    /// <param name="blogId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [HttpGet("blog-posts/{blogId}")]
    public async Task<IActionResult> GetBlogPosts(string blogId, [FromQuery] BaseQueryParams queryParams)
    {
        var response = await mediator.Send(new GetPostsByBlogQuery(blogId, queryParams));
        return StatusCode(response.StatusCode, response);
    }


    /// <summary>
    /// Updates a post
    /// </summary>
    /// <param name="post"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [HttpPut("update-post")]
    public async Task<IActionResult> UpdatePost([FromForm] UpdatePostCommand post)
    {
        var response = await mediator.Send(post);
        return StatusCode(response.StatusCode, response);
    }


    /// <summary>
    /// Deletes a post
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [HttpDelete("delete-post/{id}")]
    public async Task<IActionResult> DeletePost(string id)
    {
        var response = await mediator.Send(new DeletePostCommand(id));
        return StatusCode(response.StatusCode, response);
    }
}
