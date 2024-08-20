using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Blogs.Commands;

public class UpdateBlogCommandHandler(IUnitOfWork db) : IRequestHandler<UpdateBlogCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
    {
        var blog = await db.GetRepository<Blog>()
            .GetAsync(x => x.Id == Guid.Parse(request.BlogId), true)
            .FirstOrDefaultAsync();

        if (blog == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Blog not found");

        blog.Update(request.Title, request.Url);

        db.GetRepository<Blog>().Update(blog);

        if (await db.SaveChangesAsync())
            return ApiResponse<Guid>.Success(blog.Id, "Blog updated successfully");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, 
            "Failed to update blog, please try again later");
    }
}
