using CurriculumDomain.Features.GetCurriculumLesson.DTO.Query;
using CurriculumDomain.Features.GetStudentCurriculum.DTO.Query;
using CurriculumDomain.Features.GetStudentCurriculumDetails.DTO.Query;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Domain = CurriculumEntites.Entities;

namespace CurriculumInfrastructure.Mapping
{
    public static class MapperConfigration
    {
        public static IServiceCollection AddMapsterConfigration(this IServiceCollection services)
        {

            TypeAdapterConfig<Domain.Curriculums.Curriculum, StudentCurriculumResponseDTO>.NewConfig()
              .Map(dis => dis.Name, src => src.ShortName)
              .Map(dis => dis.BackgroundImage, src => $"https://www.selaheltelmeez.com/Media21-22/LMSAPP/SubjectIcon/slider/{src.Title}.png")
              .Map(dis => dis.Icon, src => $"https://www.selaheltelmeez.com/Media21-22/LMSAPP/SubjectIcon/coloredIcon/{src.Title}.png");

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
