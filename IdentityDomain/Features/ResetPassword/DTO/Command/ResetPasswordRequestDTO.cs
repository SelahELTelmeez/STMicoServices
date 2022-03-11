namespace IdentityDomain.Features.ResetPassword.DTO.Command;
public class ResetPasswordRequestDTO
{
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public string VerificationCode { get; set; }
    public string NewPassword { get; set; }
}