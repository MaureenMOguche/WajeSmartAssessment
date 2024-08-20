using MediatR;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Dtos;

namespace WajeSmartAssessment.Application.Features.Posts.Queries;
public record GetPostsByBlogQuery(string BlogId,
    BaseQueryParams QueryParams) : IRequest<ApiResponse>;
