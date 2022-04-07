using IdentityEntities.Entities.Identities;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityEntities.Entities.Subjects;
public class IdentitySubject : BaseEntity
{
    public DateTime CreatedOn { get; set; }
    public bool IsDeleted { get; set; }
    public Guid IdentityUserId { get; set; }
    [ForeignKey(nameof(IdentityUserId))] public IdentityUser IdentityUserFK { get; set; }
}
