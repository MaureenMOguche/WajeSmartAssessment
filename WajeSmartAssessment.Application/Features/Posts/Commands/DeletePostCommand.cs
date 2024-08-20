using MediatR;
using ReenUtility.Responses;

namespace WajeSmartAssessment.Application.Features.Posts.Commands;
public record DeletePostCommand(string PostId) : IRequest<ApiResponse>;
