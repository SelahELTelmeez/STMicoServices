namespace IdentityEntities.Entities.Identities;
public class IdentitySchool : TrackableEntity
{
    public Guid ProviderSecretKey { get; set; }
    public string Name { get; set; }
    public virtual ICollection<IdentityUser> IdentityUsers { get; set; }
}
