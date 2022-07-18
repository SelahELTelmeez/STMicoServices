namespace SharedModule.DTO;
public class NotificationRequest
{
    public string NotifiedId { get; set; }
    public string NotifierId { get; set; }
    public string Argument { get; set; }
    public int NotificationTypeId { get; set; }
    public string? AppenedMessage { get; set; }
}