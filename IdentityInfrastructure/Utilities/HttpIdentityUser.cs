using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace IdentityInfrastructure.Utilities
{
    public static class HttpIdentityUser
    {
        public static string? GetIdentityUserId(IHttpContextAccessor _accessor) => _accessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
