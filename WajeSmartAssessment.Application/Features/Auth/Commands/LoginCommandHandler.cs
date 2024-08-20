using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Services;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Auth.Commands;

public class LoginCommandHandler(UserManager<AppUser> userManager,
    ITokenHelper tokenHelper) : IRequestHandler<LoginCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var author = await userManager.FindByNameAsync(request.Username);

        if (author is null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Invalid Credentials");

        var isValidPassword = await userManager.CheckPasswordAsync(author, request.Password);

        if (!isValidPassword)
            return ApiResponse.Failure(StatusCodes.Status400BadRequest, "Invalid Credentials");

        var token = tokenHelper.GenerateLoginToken(author);

        return ApiResponse<object>.Success(new 
        { 
            UserId = author.Id.ToString(), 
            AccessToken = token 
        }, "Login successful");
    }
}
