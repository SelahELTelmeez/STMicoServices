namespace IdentityDomain.Features.UpdateProfile.DTO.Command;
public class UpdateProfileRequest
{
    public Guid? UserId { get; set; }
    public int? GradeId { get; set; }
    public string? FullName { get; set; }
    public int? AvatarId { get; set; }
    public int? CountryId { get; set; }
    public int? GovernorateId { get; set; }
    public string? DateOfBirth { get; set; }
    public int? Gender { get; set; }
    public bool? IsEmailSubscribed { get; set; }
}
