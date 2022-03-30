namespace IdentityDomain.Features.ForgetPassword.DTO.Command;
public class ForgetPasswordRequest
{
    public string? MobileNumber { get; set; }
    public string? Email { get; set; }
}