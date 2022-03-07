using IdentityEntities.Entities.Shared;

namespace IdentityEntities.Entities.Identities;
public class Avatar : BaseEntity
{
    public AvatarType AvatarType { get; set; }
    public string ImageUrl { get; set; }
}