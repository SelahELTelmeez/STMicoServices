using NotifierEntities.Entities.Invitations;
using NotifierEntities.Entities.Shared;

namespace NotifierEntities.Entities.Invitations;
public class InvitationType : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public virtual ICollection<Invitation> Invitations { get; set; }
}
