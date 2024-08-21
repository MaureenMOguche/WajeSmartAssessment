using MediatR;
using Microsoft.EntityFrameworkCore;
using ReenUtility.Extensions;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Application.Features.Posts.Dtos;
using WajeSmartAssessment.Application.Features.Posts.Extensions;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Posts.Queries;

public class GetAllPostsQueryHandler(IUnitOfWork db) : IRequestHandler<GetAllPostsQuery, ApiResponse<Paginated<PostDto>>>
{
    public async Task<ApiResponse<Paginated<PostDto>>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = db.GetRepository<Post>().GetQueryable();

        if (!string.IsNullOrEmpty(request.QueryParams.Search))
        {
            var lowerSearch = request.QueryParams.Search.ToLower();
            posts = posts.Where(p => p.Title.ToLower().Contains(lowerSearch) 
            || p.Content.ToLower().Contains(lowerSearch));
        }

        posts = posts.OrderByDescending(x => x.CreatedOn);

        var postsDto = await posts.ToPostDto().ToListAsync();

        var paginated = postsDto.Paginate(request.QueryParams.PageNumber, request.QueryParams.PageSize);

        return ApiResponse<Paginated<PostDto>>.Success(paginated, postsDto.Any() ?
            "Successfully retrieved posts" : "There are currently no posts");


    }
}