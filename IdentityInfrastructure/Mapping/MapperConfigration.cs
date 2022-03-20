using IdentityDomain.Features.GetAvatars.DTO.Query;
using IdentityDomain.Features.Login.DTO.Command;
using IdentityDomain.Features.UpdateProfile.DTO.Command;
using IdentityEntities.Entities.Identities;
using IdentityEntities.Shared.Identities;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityInfrastructure.Mapping
{
    public static class MapperConfigration
    {
        public static IServiceCollection AddMapsterConfigration(this IServiceCollection services)
        {

            TypeAdapterConfig<IdentityUser, LoginResponseDTO>.NewConfig()
                .Map(dis => dis.AvatarUrl, src => $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{Enum.GetName(typeof(AvatarType), src.AvatarFK.AvatarType)}/{src.AvatarFK.ImageUrl}")
                .Map(dis => dis.Country, src => Enum.GetName(typeof(Country), src.Country))
                .Map(dis => dis.Gender, src => Enum.GetName(typeof(Gender), src.Gender))
                .Map(dis => dis.Governorate, src => src.GovernorateFK.Name)
                .Map(dis => dis.Role, src => src.IdentityRoleFK.Name)
                .Map(dis => dis.Grade, src => src.GradeFK.Name);


            TypeAdapterConfig<IdentityUser, UpdateProfileResponseDTO>.NewConfig()
                .Map(dis => dis.AvatarUrl, src => $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{Enum.GetName(typeof(AvatarType), src.AvatarFK.AvatarType)}/{src.AvatarFK.ImageUrl}")
                .Map(dis => dis.Country, src => Enum.GetName(typeof(Country), src.Country))
                .Map(dis => dis.Gender, src => Enum.GetName(typeof(Gender), src.Gender))
                .Map(dis => dis.Governorate, src => src.GovernorateFK.Name)
                .Map(dis => dis.Role, src => src.IdentityRoleFK.Name)
                .Map(dis => dis.Grade, src => src.GradeFK.Name);



            TypeAdapterConfig<Avatar, AvatarResponseDTO>.NewConfig()
            .Map(dis => dis.ImageUrl, src => $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{Enum.GetName(typeof(AvatarType), src.AvatarType)}/{src.ImageUrl}");

            return services;
        }
    }
}
