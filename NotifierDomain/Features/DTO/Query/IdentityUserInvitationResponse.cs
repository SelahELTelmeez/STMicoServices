namespace NotifierDomain.Features.CQRS.DTO.Query;
public class IdentityUserInvitationResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Avatar { get; set; }
}