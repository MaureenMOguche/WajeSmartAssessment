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
public class GetPostsByAuthorQueryHandler(IUnitOfWork db) : IRequestHandler<GetPostsByAuthorQuery, ApiResponse>
{
    public async Task<ApiResponse> Handle(GetPostsByAuthorQuery request, CancellationToken cancellationToken)
    {
        var authorExists = await db.GetRepository<AppUser>()
            .EntityExists(user => user.Id == request.AuthorId && user.Role == UserRole.Author);

        if (!authorExists)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Author not found");

        IQueryable<Post> posts = db.GetRepository<Post>()
            .GetAsync(post => post.AuthorId == request.AuthorId)
            .Include(x => x.Author);

        if (!string.IsNullOrEmpty(request.QueryParams.Search))
        {
            var search = request.QueryParams.Search.ToLower();
            posts = posts.Where(post => post.Title.ToLower().Contains(search) ||
            post.Content.ToLower().Contains(search));
        }
            

        posts = posts.OrderByDescending(x => x.CreatedOn);

        var postsDto = await posts.ToPostDto().ToListAsync();

        var paginated = postsDto.Paginate(request.QueryParams.PageNumber, request.QueryParams.PageSize);

        return ApiResponse<Paginated<PostDto>>.Success(paginated, postsDto.Any()
            ? "Successfully retrieved blog posts" : "Blog currently has no posts");
    }
}
