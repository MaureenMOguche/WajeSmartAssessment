using MediatR;
using ReenUtility.Responses;

namespace WajeSmartAssessment.Application.Features.Posts.Queries;
public record GetPostByIdQuery(string PostId) : IRequest<ApiResponse>;
