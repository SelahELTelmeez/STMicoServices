using IdentityDomain.Features.Login.DTO.Command;
using IdentityDomain.Features.Register.DTO.Command;
using IdentityEntities.Entities.Identities;
using IdentityEntities.Entities.Shared;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityInfrastructure.Mapping
{
    public static class MapperConfigration
    {
        public static IServiceCollection AddMapsterConfigration(this IServiceCollection services)
        {

            TypeAdapterConfig<IdentityUser, IdentityLoginResponseDTO>.NewConfig()
                .Map(dis => dis.AvatarUrl, src => $"{Enum.GetName(typeof(AvatarType), src.AvatarFK.AvatarType)}/{src.AvatarFK.ImageUrl}")
                .Map(dis => dis.Country, src => Enum.GetName(typeof(Country), src.Country))
                .Map(dis => dis.Gender, src => Enum.GetName(typeof(Gender), src.Gender))
                .Map(dis => dis.Role, src => src.IdentityRoleFK.Name)
                .Map(dis => dis.Grade, src => src.GradeFK.Name);

            //TODO: Add Referral Code Generator on mapping.
            TypeAdapterConfig<IdentityRegisterRequestDTO, IdentityUser>.NewConfig()
               .Map(dis => dis.AvatarId, src => 0)
               .Map(dis => dis.Country, src => (Country)src.CountryId)
               .Map(dis => dis.Gender, src => (Gender)src.Gender)
               .Map(dis => dis.ExternalIdentityProviders, src => src.GetExternalProviders());


            TypeAdapterConfig<IdentityUser, IdentityRegisterResponseDTO>.NewConfig()
               .Map(dis => dis.AvatarUrl, src => $"{Enum.GetName(typeof(AvatarType), src.AvatarFK.AvatarType)}/{src.AvatarFK.ImageUrl}")
               .Map(dis => dis.Country, src => Enum.GetName(typeof(Country), src.Country))
               .Map(dis => dis.Gender, src => Enum.GetName(typeof(Gender), src.Gender))
               .Map(dis => dis.Role, src => src.IdentityRoleFK.Name)
               .Map(dis => dis.Grade, src => src.GradeFK.Name);

        return services;
    }
}
