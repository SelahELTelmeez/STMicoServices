using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace IdentityInfrastructure.Utilities
{
    public static class HttpIdentityExtensions
    {
        public static Guid? GetIdentityUserId(this IHttpContextAccessor _accessor) => Guid.Parse(_accessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }

    public static class HttpHeaderExtension
    {
        public static string GetAcceptLanguage(this IHttpContextAccessor _accessor, string defaultLanguage = "en-US")
        {
            string? userLangs = _accessor?.HttpContext?.Request.Headers["Accept-Language"].ToString();
            string? firstLang = userLangs?.Split(',').FirstOrDefault();
            return string.IsNullOrEmpty(firstLang) ? defaultLanguage : firstLang;
        }
    }
}
