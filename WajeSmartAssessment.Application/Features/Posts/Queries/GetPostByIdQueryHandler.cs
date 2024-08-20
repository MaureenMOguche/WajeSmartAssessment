using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Application.Features.Authors.Dtos;
using WajeSmartAssessment.Application.Features.Posts.Dtos;
using WajeSmartAssessment.Application.Features.Posts.Extensions;
using WajeSmartAssessment.Application.Helpers;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Posts.Queries;

public class GetPostByIdQueryHandler(IUnitOfWork db) : IRequestHandler<GetPostByIdQuery, ApiResponse>
{
    public async Task<ApiResponse> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await db.GetRepository<Post>()
            .GetAsync(p => p.Id == Guid.Parse(request.PostId))
            .Include(x => x.Author)
            .Include(x => x.Likes)
            .ThenInclude(x => x.User).Take(10)
            .Include(x => x.Comments).Take(10)
            .FirstOrDefaultAsync(cancellationToken);

        if (post == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, $"Post with id {request.PostId} not found");

        bool isliked = post.Likes.Any(x => x.UserId == UserHelper.GetCurrentUser()!.Id);

        var postDto = post.ToPostDetailDto(isliked);
        
        return ApiResponse<PostDetailDto>.Success(postDto, "Successfully retrieved post");
    }
}
