namespace IdentityDomain.Features.Parent.DTO;

public class AddChildInvitationRequest
{
    public int InvitationId { get; set; }
    public Guid ParentId { get; set; }
}
