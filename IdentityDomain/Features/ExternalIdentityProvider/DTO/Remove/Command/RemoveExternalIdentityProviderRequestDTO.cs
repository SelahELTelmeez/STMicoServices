namespace IdentityDomain.Features.ExternalIdentityProvider.DTO.Remove.Command;
public class RemoveExternalIdentityProviderRequestDTO
{
    public string Name { get; set; }
    public Guid IdentityUserId { get; set; }
}