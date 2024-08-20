using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Application.Contracts.Services;
using WajeSmartAssessment.Application.Helpers;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Posts.Commands;
public class CreatePostCommandHandler(IUnitOfWork db,
    IFirebaseService firebaseService) : IRequestHandler<CreatePostCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var user = UserHelper.GetCurrentUser();
        if (user is null)
            return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "Unauthorized");

        var author = await db.GetRepository<AppUser>().GetAsync(x => x.Id == user.Id && x.Role == UserRole.Author)
            .FirstOrDefaultAsync();

        if (author is null)
            return ApiResponse.Failure(StatusCodes.Status403Forbidden, "Only authors can create posts");

        if (!author.IsActive)
            return ApiResponse.Failure(StatusCodes.Status403Forbidden, "You have been disabled from making posts, please contact Admin");

        var blogExists = await db.GetRepository<Blog>().EntityExists(x => x.Id == Guid.Parse(request.BlogId));

        if (!blogExists)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Blog not found");

        List<string> media = [];
        if (request.MediaFiles != null)
        {
            var uploadedMedia = await firebaseService.UploadFiles(request.MediaFiles, "posts");
            media.AddRange(uploadedMedia);
        }

        var post = Post.Create(request.Title, request.Content, media, user.Id, Guid.Parse(request.BlogId));

        await db.GetRepository<Post>().AddAsync(post);

        if (await db.SaveChangesAsync())
            return ApiResponse<Guid>.Success(StatusCodes.Status201Created, post.Id, "Post created successfully");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to create post");
    }
}
