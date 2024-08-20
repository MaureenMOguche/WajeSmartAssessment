using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Application.Helpers;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Comments.Commands;
public class CreateCommentCommandHandler(IUnitOfWork db) : IRequestHandler<CreateCommentCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var currentUser = UserHelper.GetCurrentUser();
        if (currentUser == null)
            return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "Unauthorized");

        var postExists = await db.GetRepository<Post>()
            .GetAsync(x => x.Id == Guid.Parse(request.PostId))
            .FirstOrDefaultAsync();

        if (postExists is null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Post not found");

        var comment = Comment.Create(request.Content, currentUser.Id, postExists.BlogId);

        await db.GetRepository<Comment>().AddAsync(comment);

        if (await db.SaveChangesAsync())
            return ApiResponse<string>.Success(StatusCodes.Status201Created, comment.Id.ToString(), "Comment created successfully");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to create comment");

    }
}
