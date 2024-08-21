using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Application.Contracts.Services;
using WajeSmartAssessment.Application.Helpers;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Posts.Commands;

public class UpdatePostCommandHandler(IUnitOfWork db,
    IFirebaseService firebaseService) : IRequestHandler<UpdatePostCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var user = UserHelper.GetCurrentUser();
        if (user is null)
            return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "Unauthorized");

        List<string> media = [];
        if (request.MediaFiles != null)
        {
            var uploadedMedia = await firebaseService.UploadFiles(request.MediaFiles, "posts");
            media.AddRange(uploadedMedia);
        }

        var post = await db.GetRepository<Post>()
            .GetQueryable(x => x.Id == Guid.Parse(request.PostId), true)
            .FirstOrDefaultAsync();

        if (post is null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Post not found");

        post.Update(request.Title, request.Content, media);

        db.GetRepository<Post>().Update(post);

        if (await db.SaveChangesAsync())
            return ApiResponse<Guid>.Success(StatusCodes.Status201Created, post.Id, "Post updated successfully");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to update post");
    }
}

