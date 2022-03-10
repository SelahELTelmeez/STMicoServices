using IdentityEntities.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityEntities.Entities.Identities;
public class IdentityUserToken : BaseEntity
{
    public string Token { get; set; }
    public Guid? IdentityUserId { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? UpdatedOn { get; set; }
    [ForeignKey(nameof(IdentityUserId))] public virtual IdentityUser IdentityUserFK { get; set; }
}
