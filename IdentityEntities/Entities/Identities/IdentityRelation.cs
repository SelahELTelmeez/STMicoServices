using CurriculumEntites.Entities.Shared;
using IdentityEntities.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityEntities.Entities.Identities
{
    public class IdentityRelation : BaseEntity
    {
        public Guid AdultId { get; set; }
        public Guid KidId { get; set; }
        [ForeignKey(nameof(AdultId))] public IdentityUser Adult { get; set; }
        [ForeignKey(nameof(KidId))] public IdentityUser Kid { get; set; }
        public RelationType RelationType { get; set; }
    }
}
