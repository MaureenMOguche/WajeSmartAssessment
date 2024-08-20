using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReenUtility.Extensions;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Application.Features.Posts.Dtos;
using WajeSmartAssessment.Application.Features.Posts.Extensions;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Posts.Queries;
public class GetPostsByBlogQueryHandler(IUnitOfWork db) : IRequestHandler<GetPostsByBlogQuery, ApiResponse>
{
    public async Task<ApiResponse> Handle(GetPostsByBlogQuery request, CancellationToken cancellationToken)
    {
        var blogExists = await db.GetRepository<Blog>().EntityExists(blog => blog.Id == Guid.Parse(request.BlogId));

        if (!blogExists)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Blog not found");

        IQueryable<Post> posts = db.GetRepository<Post>()
            .GetAsync(post => post.BlogId == Guid.Parse(request.BlogId))
            .Include(x => x.Author);

        if (!string.IsNullOrEmpty(request.QueryParams.Search))
            posts = posts.Where(post => post.Title.Contains(request.QueryParams.Search, 
                StringComparison.CurrentCultureIgnoreCase));

        var postsDto = await posts.ToPostDto().ToListAsync();

        var paginated = postsDto.Paginate(request.QueryParams.PageNumber, request.QueryParams.PageSize);

        return ApiResponse<Paginated<PostDto>>.Success(paginated, postsDto.Any() 
            ? "Successfully retrieved blog posts" : "Blog currently has no posts");
    }
}
