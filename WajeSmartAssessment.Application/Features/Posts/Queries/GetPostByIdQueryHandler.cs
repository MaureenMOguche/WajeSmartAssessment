using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Application.Features.Authors.Dtos;
using WajeSmartAssessment.Application.Features.Posts.Dtos;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Posts.Queries;

public class GetPostByIdQueryHandler(IUnitOfWork db) : IRequestHandler<GetPostByIdQuery, ApiResponse>
{
    public async Task<ApiResponse> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await db.GetRepository<Post>()
            .GetAsync(p => p.Id == Guid.Parse(request.PostId))
            .Include(x => x.Author)
            .FirstOrDefaultAsync();

        if (post == null)
            return ApiResponse.Failure(StatusCodes.Status404NotFound, $"Post with id {request.PostId} not found");

        var postDto = new PostDto
        {
            Id = post.Id.ToString(),
            Title = post.Title,
            Content = post.Content,
            MediaUrls = post.MediaFiles != null ? 
                JsonConvert.DeserializeObject<List<string>>(post.MediaFiles) : new List<string>(),
            Author = new AuthorDto
            {
                Id = post.Author!.Id,
                Name = post.Author.FullName,
                Email = post.Author.Email!,
                AvatarUrl = post.Author.AvatarUrl,
            },
            BlogId = post.BlogId.ToString(),
        };

        return ApiResponse<PostDto>.Success(postDto, "Successfully retrieved post");
    }
}
