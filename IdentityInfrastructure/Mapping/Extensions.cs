using IdentityDomain.Features.Register.DTO.Command;
using IdentityEntities.Entities.Identities;

namespace IdentityInfrastructure.Mapping
{
    public static class Extensions
    {

        public static IEnumerable<ExternalIdentityProvider> GetExternalProviders(this RegisterRequestDTO requestDTO)
        {
            if (!string.IsNullOrWhiteSpace(requestDTO.GoogleId))
            {
                yield return new ExternalIdentityProvider
                {
                    Name = "Google",
                    ProviderId = requestDTO.GoogleId
                };
            }
            if (!string.IsNullOrWhiteSpace(requestDTO.FacebookId))
            {
                yield return new ExternalIdentityProvider
                {
                    Name = "Facebook",
                    ProviderId = requestDTO.FacebookId
                };
            }
            if (!string.IsNullOrWhiteSpace(requestDTO.OfficeId))
            {
                yield return new ExternalIdentityProvider
                {
                    Name = "Office",
                    ProviderId = requestDTO.OfficeId
                };
            }
        }
    }
}
