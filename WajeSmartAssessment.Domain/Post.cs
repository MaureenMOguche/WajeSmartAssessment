using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using WajeSmartAssessment.Domain.Common;

namespace WajeSmartAssessment.Domain;
public class Post : AuditableEntity
{
    protected Post() : base() { }
    private Post(string title, string content, List<string> mediaFiles, string authorId, Guid blogId)
    {
        Title = title;
        Content = content;
        MediaFiles = JsonConvert.SerializeObject(mediaFiles);
        AuthorId = authorId;
        BlogId = blogId;
    }
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? MediaFiles { get; set; }

    public string AuthorId { get; set; }

    [ForeignKey(nameof(AuthorId))]
    public AppUser? Author { get; set; }

    public Guid BlogId { get; set; }
    [ForeignKey(nameof(BlogId))]
    public Blog? Blog { get; set; }


    public static Post Create(string title, string content, List<string> mediaFiles, string authorId, Guid blogId) =>
        new Post(title, content, mediaFiles, authorId, blogId);

    public void Update(string title, string content, List<string> mediaFiles)
    {
        Title = title;
        Content = content;
        MediaFiles = JsonConvert.SerializeObject(mediaFiles);
    }
}
