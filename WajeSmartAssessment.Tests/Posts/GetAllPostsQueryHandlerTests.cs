using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WajeSmartAssessment.Application.Dtos;
using WajeSmartAssessment.Application.Features.Posts.Queries;
using WajeSmartAssessment.Domain;
using ReenUtility.Extensions;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Features.Posts.Extensions;

namespace WajeSmartAssessment.Tests.Posts;
public class GetAllPostsQueryHandlerTests : UnitOfWorkTestsSetupBase
{
    private readonly GetAllPostsQueryHandler _handler;

    public GetAllPostsQueryHandlerTests()
    {
        SetupMockRepository<Post>();

        _handler = new GetAllPostsQueryHandler(MockUnitOfWork.Object);

        var author = AppUser.Create("John", "Doe", "johndoe", "john@example.com");
        var posts = new List<Post>
        {
            Post.Create("First Post", "Content of first post", new List<string>(), author.Id, Guid.NewGuid()),
            Post.Create("Second Post", "Content of second post", new List<string>(), author.Id, Guid.NewGuid()),
            Post.Create("Third Post", "Content of third post", new List<string>(), author.Id, Guid.NewGuid()),
            Post.Create("Fourth Post", "Content of fourth post", new List<string>(), author.Id, Guid.NewGuid()),
            Post.Create("Fifth Post", "Content of fifth post", new List<string>(), author.Id, Guid.NewGuid()),
        };

        foreach (var post in posts)
        {
            AddEntity(post);
        }
    }

    [Fact]
    public async Task Handle_ReturnsAllPosts_WhenNoSearchProvided()
    {
        // Arrange
        var queryParams = new BaseQueryParams { PageNumber = 1, PageSize = 10 };

        var query = new GetAllPostsQuery(queryParams);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(5, result.Data.TotalCount);
        Assert.Equal("Successfully retrieved posts", result.Messages[0]);
    }

    [Fact]
    public async Task Handle_ReturnsFilteredPosts_WhenSearchProvided()
    {
        // Arrange
        var queryParams = new BaseQueryParams { PageNumber = 1, PageSize = 10, Search = "first" };
        var query = new GetAllPostsQuery(queryParams);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        //Assert.Single(result.Data.Data.Count);
        Assert.Contains(result.Data.Data, p => p.Title == "First Post");
    }

    [Fact]
    public async Task Handle_ReturnsPaginatedResult_WhenPageSizeIsLessThanTotalCount()
    {
        // Arrange
        var queryParams = new BaseQueryParams { PageNumber = 1, PageSize = 2 };

        var query = new GetAllPostsQuery(queryParams);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(2, result.Data.Data.Count());
        Assert.Equal(5, result.Data.TotalCount);
        Assert.Equal(3, result.Data.TotalPages);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyResult_WhenNoPostsMatch()
    {
        // Arrange
        var queryParams = new BaseQueryParams { PageNumber = 1, PageSize = 10, Search = "NonexistentPost" };

        var query = new GetAllPostsQuery(queryParams);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Empty(result.Data.Data);
        Assert.Equal("There are currently no posts", result.Messages[0]);
    }

    [Fact]
    public async Task Handle_ReturnsPostsInDescendingOrder_ByCreatedOn()
    {
        // Arrange
        var queryParams = new BaseQueryParams { PageNumber = 1, PageSize = 10 };

        var query = new GetAllPostsQuery(queryParams);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        var posts = result.Data.Data.ToList();
        for (int i = 0; i < posts.Count - 1; i++)
        {
            Assert.True(posts[i].DatePublished >= posts[i + 1].DatePublished);
        }
    }
}