namespace IdentityDomain.Features.Register.DTO.Command;
public class RegisterResponse
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string? Email { get; set; }
    public string? MobileNumber { get; set; }
    public string Grade { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string ReferralCode { get; set; }
    public bool IsPremium { get; set; }
    public string Role { get; set; }
    public string AvatarUrl { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsMobileVerified { get; set; }
}
