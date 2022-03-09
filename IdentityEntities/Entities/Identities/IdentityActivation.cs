using IdentityEntities.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityEntities.Entities.Identities
{
    public class IdentityActivation : BaseEntity
    {
        public string Code { get; set; }
        public ActivationType ActivationType { get; set; }
        public Guid IdentityUserId { get; set; }
        public bool IsVerified { get; set; }
        [ForeignKey(nameof(IdentityUserId))] public IdentityUser IdentityUserFK { get; set; }
    }
}
