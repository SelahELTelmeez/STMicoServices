using IdentityEntities.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityEntities.Entities.Identities;
public class IdentityRelation : BaseEntity
{
    public RelationType RelationType { get; set; }
    public Guid? AdultId { get; set; }    
    public Guid? KidId { get; set; }
    [ForeignKey(nameof(AdultId))] public IdentityUser AdultFK { get; set; }
    [ForeignKey(nameof(KidId))] public IdentityUser KidFK { get; set; }
}
