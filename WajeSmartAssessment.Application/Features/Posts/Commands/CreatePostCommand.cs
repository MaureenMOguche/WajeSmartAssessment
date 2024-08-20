using MediatR;
using Microsoft.AspNetCore.Http;
using ReenUtility.Responses;

namespace WajeSmartAssessment.Application.Features.Posts.Commands;
public record CreatePostCommand(
    string Title,
    string Content,
    string BlogId,
    IFormFileCollection? MediaFiles) : IRequest<ApiResponse>;
