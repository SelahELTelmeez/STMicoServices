namespace IdentityDomain.Features.ChangeEmailOrMobile.DTO.Command;
public class ChangeEmailOrMobileRequestDTO
{
    public string Password { get; set; }
    public string NewEmail { get; set; }
    public string NewMobileNumber { get; set; }
}
