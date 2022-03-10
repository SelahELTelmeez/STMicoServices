namespace IdentityDomain.Features.ForgetPassword.DTO.Command;
public class ForgetPasswordRequestDTO
{
    public string? MobileNumber { get; set; }
    public string? Email { get; set; }
}