namespace StudentDomain.Features.Shared.DTO;
public class NotificationRequest
{
    public Guid NotifiedId { get; set; }
    public Guid NotifierId { get; set; }
    public string Argument { get; set; }
    public int NotificationTypeId { get; set; }
    public string? AppenedMessage { get; set; }
}