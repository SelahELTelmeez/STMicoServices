using IdentityEntities.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityEntities.Entities.Identities;
public class ExternalIdentityProvider : BaseEntity
{
    public string Name { get; set; }
    public string IdentityProviderId { get; set; }
    public Guid IdentityUserId { get; set; }
    [ForeignKey(nameof(IdentityUserId))] public IdentityUser IdentityUserFK { get; set; }
}