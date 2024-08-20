using WajeSmartAssessment.Application.Features.Posts.Dtos;
using WajeSmartAssessment.Application.Helpers;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Contracts.Services;
public interface ICacheService
{
    Task<IEnumerable<PostDto>> FetchPostData(int pageNumber=1, int pageSize=20);
    Task<IEnumerable<Blog>> FetchBlogData(int pageNumber=1, int pageSize=20);
    void Invalidate(CacheKey key);
}
