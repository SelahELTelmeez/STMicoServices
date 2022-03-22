using System.Net.Http.Headers;

namespace ServicesGateway.Delegates
{
    public class AcceptLanguageDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AcceptLanguageDelegatingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IEnumerable<string> headerValues;
            if (request.Headers.TryGetValues("AccessToken", out headerValues))
            {
                string accessToken = headerValues.First();

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Remove("AccessToken");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }

}
