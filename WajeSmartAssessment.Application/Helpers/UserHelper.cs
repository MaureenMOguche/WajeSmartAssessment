using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace WajeSmartAssessment.Application.Helpers;

public static class UserHelper
{
    private static IHttpContextAccessor? _contextAccessor;

    public static void Configure(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public static UserPrincipal? GetCurrentUser()
    {
        if (_contextAccessor?.HttpContext?.User == null)
            return null;

        var user = new UserPrincipal
        {
            Id = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
            Username = _contextAccessor.HttpContext.User.Identity?.Name ?? string.Empty,
            Role = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty
        };

        if (string.IsNullOrEmpty(user.Id) || string.IsNullOrEmpty(user.Role))
            return null;

        return user;
    }
}
