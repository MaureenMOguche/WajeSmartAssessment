using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Authors.Commands;
public record DisableEnableAuthorCommand(string AuthorId) : IRequest<ApiResponse>;


public class DisableEnableAuthorCommandHandler(IUnitOfWork db) : IRequestHandler<DisableEnableAuthorCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DisableEnableAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await db.GetRepository<AppUser>()
            .GetAsync(user => user.Id == request.AuthorId, true)
            .FirstOrDefaultAsync();

        if (author == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Author Not Found");

        author.IsActive = !author.IsActive;
        db.GetRepository<AppUser>().Update(author);

        if (await db.SaveChangesAsync())
            return ApiResponse.Success($"Successfully {(author.IsActive ? "enabled": "disabled")} author");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, $"Failed to {(author.IsActive ? "disable" : "enable")} author");
    }
}