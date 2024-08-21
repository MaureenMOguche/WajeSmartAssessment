using MediatR;
using ReenUtility.Extensions;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Dtos;
using WajeSmartAssessment.Application.Features.Posts.Dtos;

namespace WajeSmartAssessment.Application.Features.Posts.Queries;

public record GetAllPostsQuery(BaseQueryParams QueryParams) : IRequest<ApiResponse<Paginated<PostDto>>>;
