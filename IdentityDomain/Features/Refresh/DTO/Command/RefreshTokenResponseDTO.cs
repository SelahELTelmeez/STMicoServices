namespace IdentityDomain.Features.Refresh.DTO.Command
{
    public class RefreshTokenResponseDTO
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
    }
}
