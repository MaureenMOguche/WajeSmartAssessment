using WajeSmartAssessment.Application.Dtos;

namespace WajeSmartAssessment.Application.Features.Authors.Queries;
public record GetAllAuthorsQuery(BaseQueryParams QueryParams) : IRequest<PaginatedRespon>>;
