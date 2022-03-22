using Microsoft.AspNetCore.Http;

namespace CurriculumInfrastructure.Utilities
{
    public static class HttpHeaderExtension
    {
        public static string GetAcceptLanguage(this IHttpContextAccessor _accessor, string defaultLanguage = "en-US")
        {
            string acceptLanguage = _accessor.HttpContext.Request.Headers["Accept-Language"][0];
            return (acceptLanguage == null || string.IsNullOrEmpty(acceptLanguage)) ? defaultLanguage : acceptLanguage;
        }
    }
}
