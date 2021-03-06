using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SharedModule.Extensions;
public static class HttpHeaderExtension
{
    public static string GetAcceptLanguage(this IHttpContextAccessor _accessor, string defaultLanguage = "en-US")
    {
        string? userLangs = _accessor?.HttpContext?.Request.Headers["Accept-Language"].ToString();
        string? firstLang = userLangs?.Split(',').FirstOrDefault();
        return string.IsNullOrEmpty(firstLang) ? defaultLanguage : firstLang;
    }

    public static string GetJWTToken(this IHttpContextAccessor _accessor)
        => _accessor.HttpContext.Request.Headers["Authorization"][0].Replace("Bearer", String.Empty);

    public static string? GetIdentityUserId(this IHttpContextAccessor _accessor) => _accessor.HttpContext.User.Identity.IsAuthenticated ? _accessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;
}