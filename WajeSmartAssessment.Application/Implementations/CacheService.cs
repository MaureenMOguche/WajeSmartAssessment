using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Application.Contracts.Services;
using WajeSmartAssessment.Application.Features.Posts.Dtos;
using WajeSmartAssessment.Application.Features.Posts.Extensions;
using WajeSmartAssessment.Application.Helpers;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Implementations;
public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IUnitOfWork _db;

    public CacheService(IMemoryCache memoryCache, IUnitOfWork db)
    {
        _memoryCache = memoryCache;
        _db = db;
    }

    public async Task<IEnumerable<Blog>> FetchBlogData(int pageNumber = 1, int pageSize = 20)
    {
        var skipCount = (pageNumber - 1) * pageSize;
        var cacheKey = CacheKey.Blogs.ToString();

        if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Blog>? cachedBlogs) || skipCount >= 500)
        {
            var fetchedBlogs = await FetchAbove500(cacheKey, pageSize, skipCount);
            return fetchedBlogs;
        }

        var pagedBlogs = cachedBlogs?.Skip(skipCount).Take(pageSize);
        return pagedBlogs;
    }

    public async Task<IEnumerable<PostDto>> FetchPostData(int pageNumber = 1, int pageSize = 20)
    {
        var skipCount = (pageNumber - 1) * pageSize;
        var cacheKey = CacheKey.Posts.ToString();

        if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<PostDto>? cachedPosts) || skipCount >= 500)
        {
            var fetchedPosts = await FetchPostsAbove500(cacheKey, pageSize, skipCount);
            return fetchedPosts;
        }

        var pagedPosts = cachedPosts?.Skip(skipCount).Take(pageSize);
        return pagedPosts;
    }

    public void Invalidate(CacheKey key) =>
        _memoryCache.Remove(key.ToString());

    private async Task<IEnumerable<Blog>> FetchAbove500(string cacheKey, int pageSize, int skipCount)
    {
        var fetchedBlogsQuery = _db.GetRepository<Blog>()
            .GetAsync()
            .OrderByDescending(x => x.CreatedOn);

        // Fetch and cache the first 500 items if not already cached
        if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Blog>? cachedBlogs))
        {
            var first500Blogs = await fetchedBlogsQuery.Take(500).ToListAsync();
            _memoryCache.Set(cacheKey, first500Blogs, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(15)
            });
        }

        var blogsToReturn = await fetchedBlogsQuery
            .Skip(skipCount)
            .Take(pageSize)
            .ToListAsync();

        return blogsToReturn;
    }

    private async Task<IEnumerable<PostDto>> FetchPostsAbove500(string cacheKey, int pageSize, int skipCount)
    {
        var fetchedPostsQuery = _db.GetRepository<Post>()
            .GetAsync()
            .OrderByDescending(x => x.CreatedOn);

        // Fetch and cache the first 500 items if not already cached
        if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<PostDto>? cachedPosts))
        {
            var first500Posts = await fetchedPostsQuery
                .Take(500)
                .ToPostDto()
                .ToListAsync();
            _memoryCache.Set(cacheKey, first500Posts, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(15)
            });
        }

        var postsToReturn = await fetchedPostsQuery
            .Skip(skipCount)
            .Take(pageSize)
            .ToPostDto()
            .ToListAsync();

        return postsToReturn;
    }
}
