using TransactionEntites.Entities.Shared;

namespace TransactionEntites.Entities.Invitation;

public class InvitationType : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public virtual ICollection<Invitation> Invitations { get; set; }
}
