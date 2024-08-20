using WajeSmartAssessment.Application.Features.Authors.Dtos;

namespace WajeSmartAssessment.Application.Features.Posts.Dtos;
public class PostDetailDto : PostDto
{
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public bool IsLiked { get; set; }

    public List<AuthorDto> LikedBy { get; set; } = new List<AuthorDto>();
}
