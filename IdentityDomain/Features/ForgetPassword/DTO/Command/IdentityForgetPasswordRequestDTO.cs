namespace IdentityDomain.Features.ForgetPassword.DTO.Command;
public class IdentityForgetPasswordRequestDTO
{
    public string? MobileNumber { get; set; }
    public string? Email { get; set; }
}