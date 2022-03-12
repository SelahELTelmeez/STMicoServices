namespace IdentityDomain.Features.ConfirmOTPCode.DTO.Command;
public class ConfirmOTPCodeRequestDTO
{
    public string NewPassword { get; set; }
    public string OTPCode { get; set; }
}