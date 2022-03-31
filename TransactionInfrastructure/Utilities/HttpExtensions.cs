using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace TransactionInfrastructure.Utilities
{
    public static class HttpHeaderExtension
    {
        public static string GetAcceptLanguage(this IHttpContextAccessor _accessor, string defaultLanguage = "en-US")
        {
            string acceptLanguage = _accessor.HttpContext.Request.Headers["Accept-Language"][0];
            return (acceptLanguage == null || string.IsNullOrEmpty(acceptLanguage)) ? defaultLanguage : acceptLanguage;
        }

        public static string GetJWTToken(this IHttpContextAccessor _accessor)
            => _accessor.HttpContext.Request.Headers["Authorization"][0].Replace("Bearer", String.Empty);

        public static Guid? GetIdentityUserId(this IHttpContextAccessor _accessor) => Guid.Parse(_accessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

    }
}
