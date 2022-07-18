namespace IdentityDomain.Features.ResetPassword.DTO;
public class ResetPasswordRequest
{
    public string IdentityUserId { get; set; }
    public string NewPassword { get; set; }
}
