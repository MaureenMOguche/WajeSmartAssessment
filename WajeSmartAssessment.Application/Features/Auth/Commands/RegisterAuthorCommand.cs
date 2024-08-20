using MediatR;
using Microsoft.AspNetCore.Http;
using ReenUtility.Responses;

namespace WajeSmartAssessment.Application.Features.Auth.Commands;
public record RegisterAuthorCommand(
    string FirstName,
    string LastName,
    string Username,
    string Email,
    string Password,
    IFormFile? AvatarUrl) : IRequest<ApiResponse>;
