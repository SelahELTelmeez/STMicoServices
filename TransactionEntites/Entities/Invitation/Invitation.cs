using System.ComponentModel.DataAnnotations.Schema;
using TransactionEntites.Entities.Shared;

namespace TransactionEntites.Entities.Invitation;

public class Invitation : TrackableEntity
{
    public Guid InviterId { get; set; }
    public Guid InvitedId { get; set; }
    public bool IsSeen { get; set; }
    public bool IsActive { get; set; }
    public InvitationStats Status { get; set; }
    public string Argument { get; set; }
    public int InvitationTypeId { get; set; }
    [ForeignKey(nameof(InvitationTypeId))] public InvitationType InvitationTypeFK { get; set; }
}
