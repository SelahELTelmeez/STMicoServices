using IdentityDomain.Features.Register.DTO.Command;
using IdentityEntities.Entities.Identities;

namespace IdentityInfrastructure.Mapping
{
    public static class Extensions
    {
        public static ICollection<IdentityActivation> GenerateOTP(this RegisterRequest requestDTO)
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

        public static ICollection<ExternalIdentityProvider> GetExternalProviders(this RegisterRequest requestDTO)
        {
            var providers = new List<ExternalIdentityProvider>();
            if (!string.IsNullOrWhiteSpace(requestDTO.GoogleId))
            {
                providers.Add(new ExternalIdentityProvider
                {
                    Name = "Google",
                    Identifierkey = requestDTO.GoogleId
                });
            }
            if (!string.IsNullOrWhiteSpace(requestDTO.FacebookId))
            {
                providers.Add(new ExternalIdentityProvider
                {
                    Name = "Facebook",
                    Identifierkey = requestDTO.FacebookId
                });
            }
            if (!string.IsNullOrWhiteSpace(requestDTO.OfficeId))
            {
                providers.Add(new ExternalIdentityProvider
                {
                    Name = "Office",
                    Identifierkey = requestDTO.OfficeId
                });
            }
            return providers;
        }
    }
}
