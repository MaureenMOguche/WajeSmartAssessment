namespace WajeSmartAssessment.Domain;
public class Comment
{
    protected Comment()
    {
    }
    private Comment(string content, string authorId, Guid blogId)
    {
        Id = Guid.NewGuid();
        Content = content;
        AuthorId = authorId;
        BlogId = blogId;
    }
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public AppUser? Author { get; set; }
    public Guid BlogId { get; set; }
    public Blog? Blog { get; set; }


    public static Comment Create(string content, string authorId, Guid blogId) =>
        new Comment(content, authorId, blogId);
}
