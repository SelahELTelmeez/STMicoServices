namespace TransactionDomain.Features.Invitations.CQRS.DTO.Query;

public class IdentityInvitationResponse
{
    public int InvitationId { get; set; }
    public string Description { get; set; }
    public bool IsNew { get; set; }
    public bool IsSeen { get; set; }
    public int Status { get; set; }
    public string AvatarUrl { get; set; }
    public DateTime CreatedOn { get; set; }
}
