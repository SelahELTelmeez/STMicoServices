namespace IdentityDomain.Features.ChangeEmailOrMobile.DTO.Command;
public class ChangeEmailOrMobileRequest
{
    public string Password { get; set; }
    public string? NewEmail { get; set; }
    public string? NewMobileNumber { get; set; }
}
