using MediatR;
using Microsoft.AspNetCore.Http;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Blogs.Commands;
public class CreateBlogCommandHandler(IUnitOfWork db) : IRequestHandler<CreateBlogCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
    {
        var blogExists = await db.GetRepository<Blog>()
            .EntityExists(x => x.Title.ToLower().Equals(request.Title.ToLower())
            && x.Url.Equals(request.Url.ToLower()));

        if (blogExists)
            return ApiResponse.Failure(StatusCodes.Status409Conflict, "Blog already exists");

        var blog = Blog.Create(request.Title, request.Url.ToLower());

        await db.GetRepository<Blog>().AddAsync(blog);

        if (await db.SaveChangesAsync())
            return ApiResponse<Guid>.Success(blog.Id, "Blog created successfully");

        return ApiResponse.Failure(StatusCodes.Status500InternalServerError, "Failed to create blog");
    }
}
