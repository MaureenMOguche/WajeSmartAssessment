using Newtonsoft.Json;
using WajeSmartAssessment.Application.Features.Authors.Dtos;
using WajeSmartAssessment.Application.Features.Posts.Dtos;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Posts.Extensions;
public static class PostExtensions
{
    public static IQueryable<PostDto> ToPostDto(this IQueryable<Post> posts)
    {
        var postsDto = posts.Select(post => new PostDto
        {
            Id = post.Id.ToString(),
            Title = post.Title,
            Content = post.Content,
            MediaUrls = post.MediaFiles != null ? 
                JsonConvert.DeserializeObject<List<string>>(post.MediaFiles) : new List<string>(),
            Author = new AuthorDto
            {
                Id = post.Author!.Id,
                Name = post.Author.FullName,
                Email = post.Author.Email!,
                AvatarUrl = post.Author.AvatarUrl,
            },
            BlogId = post.BlogId.ToString(),
            DatePublished = post.CreatedOn,
        });

        return postsDto;
    }

    public static PostDto ToPostDto(this Post post)
    {
        var postDto = new PostDto
        {
            Id = post.Id.ToString(),
            Title = post.Title,
            Content = post.Content,
            MediaUrls = post.MediaFiles != null ?
                JsonConvert.DeserializeObject<List<string>>(post.MediaFiles) : new List<string>(),
            Author = new AuthorDto
            {
                Id = post.Author!.Id,
                Name = post.Author.FullName,
                Email = post.Author.Email!,
                AvatarUrl = post.Author.AvatarUrl,
            },
            BlogId = post.BlogId.ToString(),
            DatePublished = post.CreatedOn,
        };

        return postDto;
    }
}
