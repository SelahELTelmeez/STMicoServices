using NotifierEntities.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotifierEntities.Entities.Invitations;
public class Invitation : TrackableEntity
{
    public Guid InviterId { get; set; }
    public Guid InvitedId { get; set; }
    public bool IsSeen { get; set; }
    public bool IsActive { get; set; }
    public InvitationStatus Status { get; set; }
    public string Message { get; set; }
    public string Title { get; set; }
    public string Argument { get; set; }
    public int InvitationTypeId { get; set; }
    [ForeignKey(nameof(InvitationTypeId))] public InvitationType InvitationTypeFK { get; set; }
}
