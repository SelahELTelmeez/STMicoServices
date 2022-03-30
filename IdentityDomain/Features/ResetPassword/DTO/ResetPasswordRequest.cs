namespace IdentityDomain.Features.ResetPassword.DTO;
public class ResetPasswordRequest
{
    public Guid IdentityUserId { get; set; }
    public string NewPassword { get; set; }
}
