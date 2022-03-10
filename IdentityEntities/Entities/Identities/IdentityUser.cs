using IdentityEntities.Entities.Locations;
using IdentityEntities.Entities.Shared;
using IdentityEntities.Shared.Identities;
using JWTGenerator.JWTModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace IdentityEntities.Entities.Identities;
public class IdentityUser
{
    [Key]
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? MobileNumber { get; set; }
    public string PasswordHash { get; set; }
    public int Grade { get; set; }
    public Gender Gender { get; set; }
    public Country Country { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastLoginOn { get; set; }
    public string? HopeToBe { get; set; }
    public string? NotificationToken { get; set; }
    public string ReferralCode { get; set; }
    public bool IsEmailSubscribed { get; set; }
    public bool IsPremium { get; set; }
    public int IdentityRoleId { get; set; }
    public int GovernorateId { get; set; }
    public int IdentitySchoolId { get; set; }
    public int AvatarId { get; set; }
    public List<RefreshToken>? RefreshTokens { get; set; }
    [ForeignKey(nameof(AvatarId))] public Avatar AvatarFK { get; set; }
    [ForeignKey(nameof(GovernorateId))] public Governorate GovernorateFK { get; set; }
    [ForeignKey(nameof(IdentityRoleId))] public IdentityRole IdentityRoleFK { get; set; }
    [ForeignKey(nameof(IdentitySchoolId))] public IdentitySchool IdentitySchoolFK { get; set; }
}
