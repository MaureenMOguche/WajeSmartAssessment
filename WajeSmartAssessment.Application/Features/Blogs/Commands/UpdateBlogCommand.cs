using MediatR;
using ReenUtility.Responses;

namespace WajeSmartAssessment.Application.Features.Blogs.Commands;
public record UpdateBlogCommand(string BlogId,
    string Title,
    string Url) : IRequest<ApiResponse>;
