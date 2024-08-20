using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Application.Contracts.Services;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Auth.Commands;
public class RegisterAuthorCommandHandler(UserManager<AppUser> userManager,
    IFirebaseService firebaseService) : IRequestHandler<RegisterAuthorCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(RegisterAuthorCommand request, CancellationToken cancellationToken)
    {
        var authorExists = await userManager.FindByEmailAsync(request.Email) is not null;

        if (authorExists)
            return ApiResponse.Failure(StatusCodes.Status409Conflict, "Author already exists");

        string? avatarUrl = null;
        if (request.AvatarUrl is not null)
            avatarUrl = await firebaseService.Upload(request.AvatarUrl, request.Username, "avatars");

        var author = AppUser.Create(
            request.FirstName,
            request.LastName,
            request.Username,
            request.Email,
            avatarUrl);

        var result = await userManager.CreateAsync(author, request.Password);

        if (!result.Succeeded)
        {
            if (avatarUrl is not null)
                BackgroundJob.Enqueue(() => firebaseService.Delete(avatarUrl));

            return ApiResponse.Failure(StatusCodes.Status400BadRequest,
                result.Errors.Select(e => e.Description).ToArray()[0]);
        }
            

        return ApiResponse.Success("Author registered successfully");

    }
}
