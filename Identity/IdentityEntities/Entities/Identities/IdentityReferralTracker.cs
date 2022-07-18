using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityEntities.Entities.Identities
{
    public class IdentityReferralTracker : TrackableEntity
    {
        public string? IdentityUserId { get; set; }
        public string? IdentityReferralUserId { get; set; }
        [ForeignKey(nameof(IdentityUserId))] public IdentityUser IdentityUserFK { get; set; }
        [ForeignKey(nameof(IdentityReferralUserId))] public IdentityUser IdentityReferralUserFK { get; set; }
    }
}
