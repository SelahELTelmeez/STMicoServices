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

            TypeAdapterConfig<Domain.Units.Unit, CurriculumUnitResponseDTO>.NewConfig()
              .Map(dis => dis.Name, src => src.ShortName); /// can not map short name in unit to be name;

            TypeAdapterConfig<Domain.Lessons.Lesson, CurriculumLessonsResponseDTO>.NewConfig()
              .Map(dis => dis.Name, src => src.ShortName);

            TypeAdapterConfig<Domain.Clips.Clip, CurriculumClipResponseDTO>.NewConfig()
              .Map(dis => dis.Name, src => src.Title);

            return services;
        }        
    }
}
