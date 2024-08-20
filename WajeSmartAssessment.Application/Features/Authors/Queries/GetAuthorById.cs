using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Application.Features.Authors.Dtos;
using WajeSmartAssessment.Application.Features.Authors.Extensions;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Authors.Queries;
public record GetAuthorByIdQuery(string authorId) : IRequest<ApiResponse>;

public class GetAuthorByIdQueryHandler(IUnitOfWork db) : IRequestHandler<GetAuthorByIdQuery, ApiResponse>
{
    public async Task<ApiResponse> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        var author = await db.GetRepository<AppUser>()
            .GetAsync(x => x.Id == request.authorId && x.Role == UserRole.Author)
            .FirstOrDefaultAsync();

        if (author == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Author not found");

        var postCounts = await db.GetRepository<Post>().GetAsync(x => x.AuthorId == author.Id).CountAsync();

        var authorDto = author.ToDto(postCounts);

        return ApiResponse<AuthorInfoDto>.Success(authorDto, "Successfully retrieved author");

    }
}


