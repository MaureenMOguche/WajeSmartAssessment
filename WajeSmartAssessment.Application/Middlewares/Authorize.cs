using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ReenUtility.Responses;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WajeSmartAssessment.Application.Middlewares
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class Authorize : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string? _policy;

        public Authorize() { }

        public Authorize(string policy)
        {
            _policy = policy;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authenticateResult = await context.HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                context.Result = new ObjectResult(new ApiResponse
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Succeeded = false,
                    Messages = [ "User is not authenticated" ]
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            SetHttpContextUser(context, authenticateResult.Principal);

            if (!string.IsNullOrEmpty(_policy))
            {
                if (context.HttpContext.User.Claims.Any(x => x.Type == ClaimTypes.Role && x.Value == _policy))
                {
                    return;
                }
                else
                {
                    context.Result = new ObjectResult(new ApiResponse
                    {
                        StatusCode = StatusCodes.Status403Forbidden,
                        Succeeded = false,
                        Messages = [ "User is not authorized to access this resource"]
                    })
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
                    return;
                }
            }

            // If no policy is specified, assume the user is authorized
            return;
        }

        private static void SetHttpContextUser(AuthorizationFilterContext context, ClaimsPrincipal principal)
        {
            context.HttpContext.User = principal;
        }
    }
}
