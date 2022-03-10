namespace IdentityDomain.Features.ChangePassword.DTO.Command;
public class ChangePasswordRequestDTO
{
    public Guid IdentityUserId { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}