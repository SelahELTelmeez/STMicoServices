namespace IdentityDomain.Features.Register.DTO.Command;
public class RegisterRequestDTO
{
    public string FullName { get; set; }
    public string PasswordHash { get; set; }
    public string? Email { get; set; }
    public string? MobileNumber { get; set; }
    public string? GoogleId { get; set; }
    public string? FacebookId { get; set; }
    public string? OfficeId { get; set; }
    public int? GradeId { get; set; }
    public int IdentityRoleId { get; set; }
}
