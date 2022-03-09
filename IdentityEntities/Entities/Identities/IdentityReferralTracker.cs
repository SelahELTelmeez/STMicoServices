using IdentityEntities.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityEntities.Entities.Identities
{
    public class IdentityReferralTracker : BaseEntity
    {
        public Guid? IdentityUserId { get; set; }
        public Guid? IdentityReferralUserId { get; set; }
        [ForeignKey(nameof(IdentityUserId))] public IdentityUser IdentityUserFK { get; set; }
        [ForeignKey(nameof(IdentityReferralUserId))] public IdentityUser IdentityReferralUserFK { get; set; }
    }
}
