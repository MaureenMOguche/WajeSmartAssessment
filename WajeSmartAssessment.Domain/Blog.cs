using WajeSmartAssessment.Domain.Common;

namespace WajeSmartAssessment.Domain;
public sealed class Blog : AuditableEntity
{
    protected Blog(): base() { }
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;

    public ICollection<Post> Posts { get; private set; } = [];

    private Blog(string title, string url)
    {
        Id = Guid.NewGuid();
        Title = title;
        Url = url;
    }

    public static Blog Create(string title, string url) =>
        new Blog(title, url);

    public void Update(string title, string url)
    {
        Title = title;
        Url = url;
    }
}
