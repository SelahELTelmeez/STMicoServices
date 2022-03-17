using CurriculumDomain.Features.GetStudentCurriculum.DTO.Query;
using Domain = CurriculumEntites.Entities;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using CurriculumDomain.Features.GetStudentCurriculumDetails.DTO.Query;
using CurriculumDomain.Features.GetCurriculumLesson.DTO.Query;

namespace CurriculumInfrastructure.Mapping
{
    public static class MapperConfigration
    {
        public static IServiceCollection AddMapsterConfigration(this IServiceCollection services)
        {

            TypeAdapterConfig<Domain.Curriculums.Curriculum, StudentCurriculumResponseDTO>.NewConfig()
              .Map(dis => dis.Name, src => src.ShortName);

            TypeAdapterConfig<Domain.Units.Unit, GetCurriculumUnitResponseDTO>.NewConfig()
              .Map(dis => dis.Name, src => src.ShortName); /// can not map short name in unit to be name

            TypeAdapterConfig<Domain.Clips.Clip, CurriculumLessonClipResponseDTO>.NewConfig()
              .Map(dis => dis.Name, src => src.Title);

            return services;
        }        
    }
}
