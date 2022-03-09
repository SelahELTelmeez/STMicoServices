namespace IdentityDomain.Features.Register.DTO.Queries;

public class IdentityCheckUserExistenceRequestDTO
{
    public string Mobile { get; set; }
    public string Email { get; set; }
    public string GoogleId { get; set; }
    public string FacebookId { get; set; }
    public string OfficeId { get; set; }
}

