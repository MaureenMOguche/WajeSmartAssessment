using MediatR;
using ReenUtility.Responses;
using WajeSmartAssessment.Application.Dtos;

namespace WajeSmartAssessment.Application.Features.Authors.Queries;
public record GetAllAuthorsQuery(BaseQueryParams QueryParams) : IRequest<ApiResponse>;


