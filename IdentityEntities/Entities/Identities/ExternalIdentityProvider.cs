using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityEntities.Entities.Identities;
public class ExternalIdentityProvider : TrackableEntity
{
    public string Identifierkey { get; set; }
    public string Name { get; set; }
    public Guid IdentityUserId { get; set; }
    [ForeignKey(nameof(IdentityUserId))] public IdentityUser IdentityUserFK { get; set; }
}