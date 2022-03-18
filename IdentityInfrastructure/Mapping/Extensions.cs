using IdentityDomain.Features.Register.DTO.Command;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;

namespace IdentityInfrastructure.Mapping
{
    public static class Extensions
    {
        public static ICollection<IdentityActivation> GenerateOTP(this RegisterRequestDTO requestDTO)
        {
            var activations = new List<IdentityActivation>();
            if (!string.IsNullOrWhiteSpace(requestDTO.Email))
            {
                activations.Add(new IdentityActivation
                {
                    ActivationType = ActivationType.Email,
                    Code = UtilityGenerator.GetOTP(4).ToString(),
                });
            }
            if (!string.IsNullOrWhiteSpace(requestDTO.MobileNumber))
            {
                activations.Add(new IdentityActivation
                {
                    ActivationType = ActivationType.Mobile,
                    Code = UtilityGenerator.GetOTP(4).ToString(),
                });
            }
            return activations;
        }

        public static ICollection<ExternalIdentityProvider> GetExternalProviders(this RegisterRequestDTO requestDTO)
        {
            var providers = new List<ExternalIdentityProvider>();
            if (!string.IsNullOrWhiteSpace(requestDTO.GoogleId))
            {
                providers.Add(new ExternalIdentityProvider
                {
                    Name = "Google",
                    ProviderId = requestDTO.GoogleId
                });
            }
            if (!string.IsNullOrWhiteSpace(requestDTO.FacebookId))
            {
                providers.Add(new ExternalIdentityProvider
                {
                    Name = "Facebook",
                    ProviderId = requestDTO.FacebookId
                });
            }
            if (!string.IsNullOrWhiteSpace(requestDTO.OfficeId))
            {
                providers.Add(new ExternalIdentityProvider
                {
                    Name = "Office",
                    ProviderId = requestDTO.OfficeId
                });
            }
            return providers;
        }
    }
}
