using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityEntities.Entities.Identities;

public class IdentityTemporaryValueHolder : TrackableEntity
{
    public string Name { get; set; }
    public string Value { get; set; }
    public string IdentityUserId { get; set; }
    [ForeignKey(nameof(IdentityUserId))] public IdentityUser IdentityUserFK { get; set; }
}
