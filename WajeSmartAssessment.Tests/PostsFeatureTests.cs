using Moq;
using Microsoft.AspNetCore.Http;
using WajeSmartAssessment.Domain;
using WajeSmartAssessment.Application.Contracts.Services;
using WajeSmartAssessment.Application.Features.Posts.Commands;
using WajeSmartAssessment.Application.Contracts.Repository;
using System.Linq.Expressions;
using WajeSmartAssessment.Infrastructure.Repositories;
using WajeSmartAssessment.Application.Helpers;
using WajeSmartAssessment.Tests.Helpers;

namespace WajeSmartAssessment.Tests;

public class CreatePostCommandHandlerTests : UnitOfWorkTestsSetupBase
{
    private AppUser author;
    private AppUser author_two;
    private Blog blog;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IFirebaseService> _mockFirebaseService;
    private readonly TestUserHelper _testUserHelper;

    private readonly Mock<IGenericRepository<Blog>> _mockBlogRepository;
    private readonly Mock<IGenericRepository<Post>> _mockPostRepository;

    public CreatePostCommandHandlerTests()
    {
        _mockFirebaseService = new Mock<IFirebaseService>();
        _testUserHelper = new TestUserHelper();

        author = AppUser.Create("John", "Doe", "johndoe", "johndoe@gmail.com");
        author_two = AppUser.Create("Jane", "Doe", "janedoe", "janedoe@gmail.com");

        blog = Blog.Create("Blog 1", "https://blog1.com");

        //Entities.AddRange(new List<Post>
        //{
        //    Post.Create("Post 1", "Content for post1", new List<string> { "https://media1.com", "https://media2.com" }, author.Id, Guid.NewGuid()),
        //    Post.Create("Post 2", "Content for post2", new List<string> { "https://media1.com", "https://media2.com" }, author.Id, Guid.NewGuid()),
        //    Post.Create("Post 3", "Content for post3", new List<string> { "https://media1.com", "https://media2.com" }, author_two.Id, Guid.NewGuid()),
        //});

        SetupMockRepository<Post>();
        SetupMockRepository<Blog>();

        //GetEntityList<Post>()
    }



    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var currentUser = _testUserHelper.SetupRegularAuthor(author.Id);

        var command = new CreatePostCommand("Test Title", "Test Content", blog.Id.ToString(), null);



        var newPost = Post.Create("Post 4", "Content for post4", ["https://media1.com", "https://media2.com"], currentUser.Id, blog.Id);
        var postRepository = MockUnitOfWork.Object.GetRepository<Post>();
        
        await postRepository.AddAsync(newPost);
        await MockUnitOfWork.Object.SaveChangesAsync();


        var blogExists = await postRepository.EntityExists(x => x.Id == Guid.Parse(command.BlogId));
        Assert.True(blogExists);


        var handler = new CreatePostCommandHandler(MockUnitOfWork.Object, _mockFirebaseService.Object);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
    }

    [Fact]
    public async Task Handle_UnauthorizedUser_ReturnsUnauthorizedResponse()
    {
        //Arrange
        _testUserHelper.SetupUnauthenticatedUser();
        var command = new CreatePostCommand("Test Title", "Test Content", Guid.NewGuid().ToString(), null);
        var handler = new CreatePostCommandHandler(MockUnitOfWork.Object, _mockFirebaseService.Object);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.Succeeded);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Handle_BlogNotFound_ReturnsNotFoundResponse()
    {
        // Arrange
        var currentUser = _testUserHelper.SetupRegularAuthor(author_two.Id);
        var command = new CreatePostCommand("Test Title", "Test Content", blog.Id.ToString(), null);

        var mockBlogRepo = MockUnitOfWork.Object.GetRepository<Blog>();
        //mockBlogRepo.Setup(repo => repo.EntityExists(It.IsAny<Expression<Func<Blog, bool>>>()))
        //    .ReturnsAsync(false);


        var handler = new CreatePostCommandHandler(MockUnitOfWork.Object, _mockFirebaseService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Handle_WithMediaFiles_UploadsFilesAndCreatesPost()
    {
        // Arrange
        var mockMediaFiles = new Mock<IFormFileCollection>();
        var command = new CreatePostCommand("Test Title", "Test Content", blog.Id.ToString(), mockMediaFiles.Object);

        var currentUser = _testUserHelper.SetupRegularAuthor(author_two.Id);
        
        var blogRepository = MockUnitOfWork.Object.GetRepository<Blog>();

        //blogRepository.EntityExists(It.IsAny<System.Linq.Expressions.Expression<Func<Blog, bool>>>()))
        
            //_mockUnitOfWork.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(true);
        _mockFirebaseService.Setup(service => service.UploadFiles(It.IsAny<IFormFileCollection>(), It.IsAny<string>()))
            .ReturnsAsync(new List<string> { "file1.jpg", "file2.jpg" });

        var handler = new CreatePostCommandHandler(MockUnitOfWork.Object, _mockFirebaseService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _mockFirebaseService.Verify(service => service.UploadFiles(It.IsAny<IFormFileCollection>(), "posts"), Times.Once);
        _mockPostRepository.Verify(repo => repo.AddAsync(It.Is<Post>(p => p.MediaFiles!.Count() == 2)), Times.Once);
    }

}


