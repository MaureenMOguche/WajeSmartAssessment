using WajeSmartAssessment.Application.Features.Authors.Dtos;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Posts.Dtos;
public class PostDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string>? MediaUrls { get;  set; }
    public DateTime DatePublished { get; set; }
    public string BlogId { get; set; } = string.Empty;
    public AuthorDto? Author { get; set; }
}