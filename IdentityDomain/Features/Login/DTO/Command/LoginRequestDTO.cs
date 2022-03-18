namespace IdentityDomain.Features.Login.DTO.Command;
public class LoginRequestDTO
{
    public string? MobileNumber { get; set; }
    public string? Email { get; set; }
    public string PasswordHash { get; set; }
    public string? GoogleId { get; set; }
    public string? FacebookId { get; set; }
    public string? OfficeId { get; set; }
}
