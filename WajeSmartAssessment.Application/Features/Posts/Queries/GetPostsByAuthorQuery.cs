using MediatR;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Dtos;

namespace WajeSmartAssessment.Application.Features.Posts.Queries;
public record GetPostsByAuthorQuery(string AuthorId, BaseQueryParams QueryParams) :IRequest<ApiResponse>;
