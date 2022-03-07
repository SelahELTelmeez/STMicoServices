using IdentityEntities.Entities.Shared;

namespace IdentityEntities.Entities.Locations
{
    public class Governorate : BaseEntity
    {
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
    }
}
