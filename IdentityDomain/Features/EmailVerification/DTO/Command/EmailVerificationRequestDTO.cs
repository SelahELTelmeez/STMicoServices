namespace IdentityDomain.Features.EmailVerification.DTO.Command;
public class EmailVerificationRequestDTO
{
    public string Email { get; set; }
    public string Code { get; set; }
}
