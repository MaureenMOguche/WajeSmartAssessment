using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Blogs.Commands;

public class DeleteBlogCommandHandler(IUnitOfWork db) : IRequestHandler<DeleteBlogCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DeleteBlogCommand request, CancellationToken cancellationToken)
    {
        var blog = await db.GetRepository<Blog>()
            .GetAsync(b => b.Id == Guid.Parse(request.BlogId), true)
            .FirstOrDefaultAsync();

        if (blog == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, "Blog not found");

        db.GetRepository<Blog>().Delete(blog);

        if (await db.SaveChangesAsync())
            return ApiResponse.Success("Blog deleted successfully");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to delete blog");
    }
}
