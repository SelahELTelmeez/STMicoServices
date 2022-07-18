namespace IdentityDomain.Features.Integration.DTO;

public class ExternalUserRegisterRequest
{
    public string ExternalUserId { get; set; } = default!;
    public int RoleId { get; set; }
    public string FullName { get; set; } = default!;
    public int? GradeId { get; set; }
    public string? Email { get; set; }
    public string? MobileNumber { get; set; }
}
