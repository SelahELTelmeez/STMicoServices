namespace NotifierDomain.Features.CQRS.DTO.Query;
public class InvitationResponse
{
    public int InvitationId { get; set; }
    public string InviterId { get; set; }
    public string InvitedId { get; set; }
    public string Description { get; set; }
    public bool IsSeen { get; set; }
    public int Status { get; set; }
    public string AvatarUrl { get; set; }
    public string Argument { get; set; }
    public DateTime CreatedOn { get; set; }
    public int Type { get; set; }
}
