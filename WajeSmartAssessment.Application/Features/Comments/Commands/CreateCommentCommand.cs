using MediatR;
using ReenUtility.Responses;

namespace WajeSmartAssessment.Application.Features.Comments.Commands;
public record CreateCommentCommand(string PostId, string Content) : IRequest<ApiResponse>;
