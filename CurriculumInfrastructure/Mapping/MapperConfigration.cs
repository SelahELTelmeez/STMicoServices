using CurriculumDomain.Features.GetStudentCurriculum.DTO.Query;
using CurriculumEntites.Entities.Curriculums;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace CurriculumInfrastructure.Mapping
{
    public static class MapperConfigration
    {
        public static IServiceCollection AddMapsterConfigration(this IServiceCollection services)
        {

            TypeAdapterConfig<Curriculum, StudentCurriculumResponseDTO>.NewConfig()
               .Map(dis => dis.Name, src => src.ShortName);


            return services;
        }
    }
}
