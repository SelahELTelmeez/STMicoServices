namespace IdentityDomain.Features.IdentityUserTransaction.DTO
{
    public class SearchOnStudentResponse
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? GradeName { get; set; }
        public string? AvatarImage { get; set; }
    }
}