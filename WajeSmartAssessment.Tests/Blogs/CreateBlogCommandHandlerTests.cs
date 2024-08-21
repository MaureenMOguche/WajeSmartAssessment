using Microsoft.AspNetCore.Http;
using Moq;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Features.Blogs.Commands;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Tests.Blogs;
public class CreateBlogCommandHandlerTests : UnitOfWorkTestsSetupBase
{
    private readonly CreateBlogCommandHandler _handler;

    public CreateBlogCommandHandlerTests()
    {
        SetupMockRepository<Blog>();
        _handler = new CreateBlogCommandHandler(MockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var command = new CreateBlogCommand("Test Blog", "https://testblog.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.IsType<Guid>(((ApiResponse<Guid>)result).Data);
        Assert.Equal("Blog created successfully", result.Messages[0]);

        VerifySaveChangesAsyncCalled(Times.Once());

        var addedBlog = GetEntityList<Blog>().Find(b => b.Title == command.Title && b.Url == command.Url.ToLower());
        Assert.NotNull(addedBlog);
    }

    [Fact]
    public async Task Handle_DuplicateBlog_ReturnsConflictResponse()
    {
        // Arrange
        var existingBlog = Blog.Create("Existing Blog", "https://existingblog.com");
        AddEntity(existingBlog);

        var command = new CreateBlogCommand("Existing Blog", "https://existingblog.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);
        Assert.Equal("Blog already exists", result.Messages[0]);

        VerifySaveChangesAsyncCalled(Times.Never());
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ReturnsInternalServerErrorResponse()
    {
        // Arrange
        SetupSaveChangesAsync(false); // Setup SaveChangesAsync to return false

        var command = new CreateBlogCommand("Test Blog", "https://testblog.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.Equal("Failed to create blog", result.Messages[0]);

        VerifySaveChangesAsyncCalled(Times.Once());
    }
}