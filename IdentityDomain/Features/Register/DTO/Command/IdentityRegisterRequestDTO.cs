namespace IdentityDomain.Features.Register.DTO.Command;
public class IdentityRegisterRequestDTO
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string GoogleId { get; set; }
    public string FacebookId { get; set; }
    public string PasswordHash { get; set; }
    public string StudingYear { get; set; }
    public string UserRoleId { get; set; }
    public string Mobile { get; set; }
    public string CountryId { get; set; }
    public string GovernorateId { get; set; }
    public string YOB { get; set; }
    public string Gender { get; set; }
    public string SendEmail { get; set; }
}
