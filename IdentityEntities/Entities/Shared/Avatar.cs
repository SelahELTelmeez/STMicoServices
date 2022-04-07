namespace IdentityEntities.Shared.Identities;
public class Avatar : BaseEntity
{
    public AvatarType AvatarType { get; set; }
    public string ImageUrl { get; set; }
}