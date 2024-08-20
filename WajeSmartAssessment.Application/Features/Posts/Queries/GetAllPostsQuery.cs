using MediatR;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Dtos;

namespace WajeSmartAssessment.Application.Features.Posts.Queries;

public record GetAllPostsQuery(BaseQueryParams QueryParams) : IRequest<ApiResponse>;
