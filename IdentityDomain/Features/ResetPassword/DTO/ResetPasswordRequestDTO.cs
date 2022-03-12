namespace IdentityDomain.Features.ResetPassword.DTO;
public class ResetPasswordRequestDTO
{
    public Guid IdentityUserId { get; set; }
    public string NewPassword { get; set; }
}
