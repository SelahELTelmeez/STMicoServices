namespace IdentityDomain.Features.ExternalIdentityProvider.DTO.Add.Command;
public class AddExternalIdentityProviderRequestDTO
{
    public string Name { get; set; }
    public Guid IdentityUserId { get; set; }
}