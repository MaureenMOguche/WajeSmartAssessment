using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Application.Features.Blogs.Validations;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Blogs.Queries;

public class GetBlogByIdQueryHandler(IUnitOfWork db) : IRequestHandler<GetBlogByIdQuery, ApiResponse>
{
    public async Task<ApiResponse> Handle(GetBlogByIdQuery request, CancellationToken cancellationToken)
    {
        var validator = new GetBlogByIdQueryValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
            return ApiResponse.Failure(StatusCodes.Status400BadRequest, string.Join(", ", result.Errors));

        var blog = await db.GetRepository<Blog>()
            .GetQueryable(b => b.Id == Guid.Parse(request.BlogId))
            .FirstOrDefaultAsync();

        if (blog == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Blog not found");

        return ApiResponse<Blog>.Success(blog, "Blog retrieved successfully");
    }
}
