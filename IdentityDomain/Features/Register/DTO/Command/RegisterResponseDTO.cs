namespace IdentityDomain.Features.Register.DTO.Command;
public class RegisterResponseDTO
{
    public string? Email { get; set; }
    public string? MobileNumber { get; set; }
    public string Grade { get; set; }
    public string Gender { get; set; }
    public string Country { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string ReferralCode { get; set; }
    public bool IsPremium { get; set; }
    public string Role { get; set; }
    public string Governorate { get; set; }
    public string AvatarUrl { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
