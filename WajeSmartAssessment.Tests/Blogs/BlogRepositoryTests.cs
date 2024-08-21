using Moq;
using Microsoft.EntityFrameworkCore;
using WajeSmartAssessment.Domain;
using WajeSmartAssessment.Infrastructure.Repositories;
using WajeSmartAssessment.Infrastructure;

public class BlogRepositoryTests : RepositoryTestsSetupBase<Blog>
{
    public BlogRepositoryTests() : base()
    {
        Entities.AddRange(new List<Blog>
        {
            Blog.Create("Blog 1", "https://blog1.com"),
            Blog.Create("Blog 2", "https://blog2.com"),
            Blog.Create("Blog 3", "https://blog3.com")
        });

        SetupMockDbSet();
    }


    [Fact]
    public async Task AddBlog_AddsBlogToDatabase()
    {
        // Arrange
        var blogRepository = UnitOfWork.GetRepository<Blog>();

        // Act
        var newBlog = Blog.Create("Blog 4", "https://blog4.com");
        Assert.NotNull(newBlog); // Ensure the blog is created successfully

        await blogRepository.AddAsync(newBlog);
        await UnitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(4, Entities.Count);
        Assert.Contains(Entities, b => b.Title == "Blog 4" && b.Url == "https://blog4.com");
    }

    [Fact]
    public void GetBlogs_ReturnsAllBlogs()
    {
        // Arrange
        var blogRepository = UnitOfWork.GetRepository<Blog>();

        // Act
        var result = blogRepository.GetQueryable();

        // Assert
        Assert.Equal(3, result.Count());
        Assert.True(result.Any(b => b.Title == "Blog 1"));
        Assert.True(result.Any(b => b.Title == "Blog 2"));
        Assert.True(result.Any(b => b.Title == "Blog 3"));
    }

    [Fact]
    public async Task DeleteBlog_RemovesBlogFromDatabase()
    {
        // Arrange
        var blogRepository = UnitOfWork.GetRepository<Blog>();

        var blogCount = Entities.Count;

        // Act
        var blogToDelete = Entities.First();
        blogRepository.Delete(blogToDelete);
        await UnitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(blogCount - 1, Entities.Count);
        Assert.DoesNotContain(Entities, b => b.Title == blogToDelete.Title);
    }
}