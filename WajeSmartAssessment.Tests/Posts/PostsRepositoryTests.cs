using Moq;
using Microsoft.EntityFrameworkCore;
using WajeSmartAssessment.Domain;
using WajeSmartAssessment.Infrastructure.Repositories;
using WajeSmartAssessment.Infrastructure;

public class PostsRepostioryTests : RepositoryTestsSetupBase<Post>
{
    private AppUser author;
    private Blog blog;
    public PostsRepostioryTests() : base()
    {
        author = AppUser.Create("John", "Doe", "johndoe", "johmdoe@gmail.com");
        blog = Blog.Create("Blog 1", "https://blog1.com");

        Entities.AddRange(new List<Post>
        {
            Post.Create("Post 1", "Content for post1", ["https://media1.com","https://media2.com" ], author.Id, blog.Id),
            Post.Create("Post 2", "Content for post2", ["https://media1.com","https://media2.com" ], author.Id, blog.Id),
            Post.Create("Post 3", "Content for post3", ["https://media1.com","https://media2.com" ], author.Id, blog.Id),
        });

        SetupMockDbSet();
    }


    [Fact]
    public async Task AddPost_AddsPostToDatabase()
    {
        // Arrange
        var postRepository = UnitOfWork.GetRepository<Post>();

        // Act
        var newPost = Post.Create("Post 4", "Content for post4", ["https://media1.com", "https://media2.com"], author.Id, blog.Id);
        Assert.NotNull(newPost);

        await postRepository.AddAsync(newPost);
        await UnitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(4, Entities.Count);
        Assert.Contains(Entities, b => b.Title == "Post 4" && b.Content == "Content for post4");
    }

    [Fact]
    public void GetPosts_ReturnsAllPosts()
    {
        // Arrange
        var postRepository = UnitOfWork.GetRepository<Post>();

        // Act
        var result = postRepository.GetQueryable();

        // Assert
        Assert.Equal(3, result.Count());
        Assert.True(result.Any(b => b.Title == "Post 1"));
        Assert.True(result.Any(b => b.Title == "Post 2"));
        Assert.True(result.Any(b => b.Title == "Post 3"));
    }

    [Fact]
    public async Task DeletePost_RemovesPostFromDatabase()
    {
        // Arrange
        var postRepository = UnitOfWork.GetRepository<Post>();

        var postCount = Entities.Count;

        // Act
        var postToDelete = Entities.First();
        postRepository.Delete(postToDelete);
        await UnitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(postCount - 1, Entities.Count);
        Assert.DoesNotContain(Entities, b => b.Title == postToDelete.Title);
    }
}