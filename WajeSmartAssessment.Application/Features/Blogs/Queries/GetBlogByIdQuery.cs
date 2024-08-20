using MediatR;
using ReenUtility.Responses;

namespace WajeSmartAssessment.Application.Features.Blogs.Queries;
public record GetBlogByIdQuery(string BlogId) : IRequest<ApiResponse>;
