namespace SharedModule.DTO;

public class InvitationRequest
{
    public Guid InviterId { get; set; }
    public Guid InvitedId { get; set; }
    public int InvitationTypeId { get; set; }
    public string? Argument { get; set; }
    public string? AppenedMessage { get; set; }
    public bool IsActive { get; set; }
}
