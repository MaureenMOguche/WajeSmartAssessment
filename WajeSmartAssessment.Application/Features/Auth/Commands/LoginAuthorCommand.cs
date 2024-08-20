using MediatR;
using ReenUtility.Responses;

namespace WajeSmartAssessment.Application.Features.Auth.Commands;
public record LoginCommand(string Username, string Password) : IRequest<ApiResponse>;
