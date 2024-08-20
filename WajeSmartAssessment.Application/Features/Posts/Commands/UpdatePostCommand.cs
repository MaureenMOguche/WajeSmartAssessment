using MediatR;
using Microsoft.AspNetCore.Http;
using ReenUtility.Responses;

namespace WajeSmartAssessment.Application.Features.Posts.Commands;
public record UpdatePostCommand(string PostId, 
    string Title, 
    string Content,
    IFormFileCollection? MediaFiles) : IRequest<ApiResponse>;

