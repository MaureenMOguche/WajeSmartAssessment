using MediatR;
using Microsoft.EntityFrameworkCore;
using ReenUtility.Extensions;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Blogs.Queries;

public class GetAllBlogsQueryHandler(IUnitOfWork db) : IRequestHandler<GetAllBlogsQuery, ApiResponse>
{
    public async Task<ApiResponse> Handle(GetAllBlogsQuery request, CancellationToken cancellationToken)
    {
        var blogs = db.GetRepository<Blog>().GetAsync();

        if (!string.IsNullOrEmpty(request.QueryParams.Search))
            blogs = blogs.Where(blog => blog.Title.Contains(request.QueryParams.Search, 
                StringComparison.CurrentCultureIgnoreCase));

        var blogList = await blogs.ToListAsync();

        var paginated = blogList.Paginate(request.QueryParams.PageNumber, request.QueryParams.PageSize);

        return ApiResponse<Paginated<Blog>>.Success(paginated, blogs.Any() ? 
            "Successfully retrieved blogs" : "There are currently no blogs");

    }
}
