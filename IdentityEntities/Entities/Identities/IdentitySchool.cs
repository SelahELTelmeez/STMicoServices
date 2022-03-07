using IdentityEntities.Entities.Shared;

namespace IdentityEntities.Entities.Identities;
public class IdentitySchool : BaseEntity
{
    public string Name { get; set; }
    public virtual ICollection<IdentityUser> IdentityUsers { get; set; }
}
