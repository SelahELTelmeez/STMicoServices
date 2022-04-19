namespace IdentityDomain.Features.IdentityUserTransaction.DTO
{
    public class AddNewChildResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Grade { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ReferralCode { get; set; }
        public bool IsPremium { get; set; }
        public string Role { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}