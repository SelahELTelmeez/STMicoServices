namespace IdentityDomain.Features.Refresh.DTO.Command
{
    public class RefreshTokenResponse
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
    }
}
