using IdentityDomain.Features.Login.DTO.Command;
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
                .Map(dis => dis.Role, src => src.IdentityRoleFK.Name);

            return services;
        }
    }
}
