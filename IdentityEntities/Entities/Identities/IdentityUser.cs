using CurriculumEntites.Entities.Shared;
using IdentityEntities.Entities.Locations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityEntities.Entities.Identities
{
    public class IdentityUser
    {
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
        public int IdentityRole { get; set; }
        public string? HopeToBe { get; set; }
        public string? NotificationToken { get; set; }
        public string ReferralCode { get; set; }
        public bool IsEmailSubscribed { get; set; }
        public int Governorate { get; set; }
        [ForeignKey(nameof(Governorate))] public Governorate GovernorateFK { get; set; }
        [ForeignKey(nameof(IdentityRole))] public IdentityRole IdentityRoleFK { get; set; }
    }
}
