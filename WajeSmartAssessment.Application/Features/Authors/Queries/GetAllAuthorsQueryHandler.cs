using MediatR;
using ReenUtility.Extensions;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Application.Features.Authors.Dtos;
using WajeSmartAssessment.Application.Features.Authors.Extensions;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Authors.Queries;


public class GetAllAuthorsQueryHandler(IUnitOfWork db) : IRequestHandler<GetAllAuthorsQuery, ApiResponse>
{
    public async Task<ApiResponse> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
{
    var authors = db.GetRepository<AppUser>().GetQueryable(user => user.Role == UserRole.Author);

    if (!string.IsNullOrEmpty(request.QueryParams.Search))
    {
        var search = request.QueryParams.Search.ToLower();
        authors = authors.Where(user => user.FirstName.ToLower().Contains(search)
        || user.LastName.ToLower().Contains(search));
    }

    var authorsCount = authors.Count();

    var pagedAuthors = authors.Skip((request.QueryParams.PageNumber - 1) * request.QueryParams.PageSize)
                      .Take(request.QueryParams.PageSize)
                      .ToList();


    var authorIds = pagedAuthors.Select(user => user.Id).ToList();

    var postCounts = db.GetRepository<Post>().GetQueryable(post => authorIds.Contains(post.AuthorId))
               .GroupBy(post => post.AuthorId)
               .Select(group => new
               {
                   AuthorId = group.Key,
                   PostCount = group.Count()
               })
               .ToDictionary(g => g.AuthorId, g => g.PostCount);


    var authorsDto = pagedAuthors.Select(author =>
    {
        var postCount = postCounts.ContainsKey(author.Id) ? postCounts[author.Id] : 0;
        return author.ToDto(postCount);
    });

    var paginate = new Paginated<AuthorDto>(authorsDto, authorsCount, request.QueryParams.PageNumber, request.QueryParams.PageSize);

    return ApiResponse<Paginated<AuthorDto>>.Success(paginate, "Authors retrieved successfully");
}
}