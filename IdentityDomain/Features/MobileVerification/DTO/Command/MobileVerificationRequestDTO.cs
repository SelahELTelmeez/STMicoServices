namespace IdentityDomain.Features.MobileVerification.DTO.Command;
public class MobileVerificationRequestDTO
{
    public Guid IdentityUserId { get; set; }
    public string MobileNumber { get; set; }
    public string Code { get; set; }
}
