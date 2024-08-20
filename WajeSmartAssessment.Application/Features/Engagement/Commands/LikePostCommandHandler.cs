using MediatR;
using Microsoft.AspNetCore.Http;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Application.Helpers;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Engagement.Commands;

public class LikePostCommandHandler(IUnitOfWork db) : IRequestHandler<LikePostCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(LikePostCommand request, CancellationToken cancellationToken)
    {
        var currentUser = UserHelper.GetCurrentUser();

        if (currentUser == null)
            return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "Unauthorized");

        var postExist = await db.GetRepository<Post>().EntityExists(p => p.Id == Guid.Parse(request.PostId));

        if (!postExist)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Post not found");

        var likeExist = await db.GetRepository<Like>()
            .EntityExists(l => l.PostId == Guid.Parse(request.PostId) && l.UserId == request.UserId);

        if (likeExist)
            return ApiResponse.Failure(StatusCodes.Status400BadRequest, "You have already liked this post");

        var like = Like.LikePost(request.PostId, currentUser.Id);

        await db.GetRepository<Like>().AddAsync(like);

        if (await db.SaveChangesAsync())
            return ApiResponse.Success("Successfully liked post");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to like post");
    }
}