namespace IdentityDomain.Features.ChangePassword.DTO.Command;
public class ChangePasswordRequest
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}