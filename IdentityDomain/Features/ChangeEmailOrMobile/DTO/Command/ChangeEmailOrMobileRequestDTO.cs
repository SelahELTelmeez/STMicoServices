namespace IdentityDomain.Features.ChangeEmailOrMobile.DTO.Command;
public class ChangeEmailOrMobileRequestDTO
{
    public Guid IdentityUserId { get; set; }
    public string Password { get; set; }
    public string NewEmail { get; set; }
    public string NewMobileNumber { get; set; }
}
