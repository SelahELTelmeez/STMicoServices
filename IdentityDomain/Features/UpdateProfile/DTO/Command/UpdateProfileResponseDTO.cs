namespace IdentityDomain.Features.UpdateProfile.DTO.Command;

public class UpdateProfileResponseDTO
{
    public string FullName { get; set; }
    public string? Email { get; set; }
    public string? MobileNumber { get; set; }
    public string? Grade { get; set; }
    public string? Gender { get; set; }
    public string? Country { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string ReferralCode { get; set; }
    public bool IsPremium { get; set; }
    public string Role { get; set; }
    public string? Governorate { get; set; }
    public string AvatarUrl { get; set; }
    public bool IsVerified { get; set; }
}
