using MediatR;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Dtos;

namespace WajeSmartAssessment.Application.Features.Blogs.Queries;
public record GetAllBlogsQuery(BaseQueryParams QueryParams) : IRequest<ApiResponse>;
