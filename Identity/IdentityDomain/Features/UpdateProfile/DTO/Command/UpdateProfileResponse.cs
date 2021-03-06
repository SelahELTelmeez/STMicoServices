namespace IdentityDomain.Features.UpdateProfile.DTO.Command;

public class UpdateProfileResponse
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string? Email { get; set; }
    public string? MobileNumber { get; set; }
    public string? Grade { get; set; }
    public int? GradeId { get; set; }
    public int RoleId { get; set; }
    public string? Gender { get; set; }
    public string? Country { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string ReferralCode { get; set; }
    public bool IsPremium { get; set; }
    public string Role { get; set; }
    public string? Governorate { get; set; }
    public string AvatarUrl { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsMobileVerified { get; set; }
}
