using Newtonsoft.Json;
using WajeSmartAssessment.Application.Features.Authors.Extensions;
using WajeSmartAssessment.Application.Features.Posts.Dtos;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Posts.Extensions;
public static class PostDetailExtensions
{
    public static PostDetailDto ToPostDetailDto(this Post post, bool isLiked) =>
        new PostDetailDto
        {
            Id = post.Id.ToString(),
            Title = post.Title,
            Content = post.Content,
            DatePublished = post.CreatedOn,
            Author = post.Author?.ToDto(),
            LikeCount = post.Likes.Count,
            CommentCount = post.Comments.Count,
            MediaUrls = post.MediaFiles != null ? JsonConvert.DeserializeObject<List<string>>(post.MediaFiles) : new List<string>(),
            LikedBy = post.Likes.Select(x => x.User.ToDto()).ToList(),
            BlogId = post.BlogId.ToString(),
            IsLiked = isLiked
        };
}
