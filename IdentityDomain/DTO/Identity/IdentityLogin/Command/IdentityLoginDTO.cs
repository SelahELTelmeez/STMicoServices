namespace IdentityDomain.DTO.Identity.IdentityLogin.Command;
public class IdentityLoginDTO
{
    public string Mobile { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string GoogleId { get; set; }
    public string FacebookId { get; set; }
    public string OfficeId { get; set; }
}
