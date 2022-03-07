using IdentityEntities.Entities.Identities;
using IdentityEntities.Entities.Shared;

namespace IdentityEntities.Entities.Subjects
{
    public class IdentitySubject : BaseEntity
    {
        public DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public Guid IdentityUserId { get; set; }
        public IdentityUser IdentityUserFK { get; set; }
    }
}
