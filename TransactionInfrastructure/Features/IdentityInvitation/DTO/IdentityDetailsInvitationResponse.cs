namespace TransactionInfrastructure.Features.IdentityInvitation.DTO
{
    public class IdentityDetailsInvitationResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        public string? NotificationToken { get; set; }
        public bool IsPremium { get; set; }
        public int? AvatarId { get; set; }
    }
}
