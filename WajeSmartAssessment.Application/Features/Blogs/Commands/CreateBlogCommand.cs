using MediatR;
using ReenUtility.Responses;

namespace WajeSmartAssessment.Application.Features.Blogs.Commands;
public record CreateBlogCommand(
    string Title,
    string Url) : IRequest<ApiResponse>;
