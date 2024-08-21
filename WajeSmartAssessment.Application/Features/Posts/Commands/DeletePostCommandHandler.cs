using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Application.Helpers;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Posts.Commands;

public class DeletePostCommandHandler(IUnitOfWork db) : IRequestHandler<DeletePostCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var currentUser = UserHelper.GetCurrentUser();

        if (currentUser == null)
            return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "Unauthorized access.");

        var post = await db.GetRepository<Post>()
            .GetQueryable(p => p.Id == Guid.Parse(request.PostId), true)
            .FirstOrDefaultAsync();

        if (post == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Post not found.");

        if (post.AuthorId != currentUser.Id)
            return ApiResponse.Failure(StatusCodes.Status403Forbidden, "You are not authorized to delete this post.");

        db.GetRepository<Post>().Delete(post);

        if (await db.SaveChangesAsync())
            return ApiResponse.Success("Post successfully delete.");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, 
            "An error occurred while deleting post, please try again.");
    }
}
