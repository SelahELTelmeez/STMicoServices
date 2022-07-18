using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityEntities.Entities.Identities;
public class IdentityRelation : TrackableEntity
{
    public RelationType RelationType { get; set; }
    public string? PrimaryId { get; set; }
    public string? SecondaryId { get; set; }
    [ForeignKey(nameof(PrimaryId))] public IdentityUser PrimaryFK { get; set; }
    [ForeignKey(nameof(SecondaryId))] public IdentityUser SecondaryFK { get; set; }
}
