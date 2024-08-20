using MediatR;
using ReenUtility.Responses;

namespace WajeSmartAssessment.Application.Features.Blogs.Commands;
public record DeleteBlogCommand(string BlogId) : IRequest<ApiResponse>;
