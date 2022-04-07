using IdentityDomain.Features.IdentityAvatars.DTO.Query;
using IdentityEntities.Shared.Identities;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityInfrastructure.Mapping;
public static class MapperConfigration
{
    public static IServiceCollection AddMapsterConfigration(this IServiceCollection services)
    {

        TypeAdapterConfig<Avatar, IdentityAvatarResponse>.NewConfig()
        .Map(dis => dis.ImageUrl, src => $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{Enum.GetName(typeof(AvatarType), src.AvatarType)}/{src.ImageUrl}");

        return services;
    }
}