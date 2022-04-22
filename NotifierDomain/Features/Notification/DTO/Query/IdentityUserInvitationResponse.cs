namespace NotifierDomain.Features.Invitations.CQRS.DTO.Query;
public class IdentityUserNotificationResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Avatar { get; set; }
}