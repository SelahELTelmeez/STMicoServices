using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace IdentityInfrastructure.Utilities
{
    public static class HttpIdentityUser
    {
        public static Guid? GetIdentityUserId(IHttpContextAccessor _accessor) => Guid.Parse(_accessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }
}
