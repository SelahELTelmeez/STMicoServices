namespace IdentityDomain.Features.ExternalIdentityProvider.DTO.Add.Command;
public class AddExternalIdentityProviderRequest
{
    public string ProviderId { get; set; }
    public string Name { get; set; }
}