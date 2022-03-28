using CurriculumDomain.Features.GetCurriculumLesson.DTO.Query;
using CurriculumDomain.Features.GetStudentCurriculum.DTO.Query;
using CurriculumDomain.Features.GetStudentCurriculumDetails.DTO.Query;
using CurriculumEntites.Entities.Shared;
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
              .Map(dis => dis.ClipName, src => src.Title)
              .Map(dis => dis.GameObjectUrl, src => getGameObjectUrl(src.Type, src.LessonFK.UnitFK.CurriculumFK.Id, src.FileName))
              .Map(dis => dis.Thumbnail, src => getThumbnailUrl(src.Type, src.LessonFK.UnitFK.CurriculumFK.Id, src.Id));

            TypeAdapterConfig<Domain.Clips.Clip, FilterTypesResponseDTO>.NewConfig()
               .Map(dis => dis.ImageUrl, src => $"https://www.selaheltelmeez.com/Media21-22/LMSApp/FilterImage/{src.Type}.png")
               .Map(dis => dis.Name, src => Enum.GetName(typeof(ClipType), src.Type.GetValueOrDefault()));


            return services;
        }
        private static string getGameObjectUrl(ClipType? clipType, string CurriculumId, string fileName)
        {
            return clipType switch
            {
                ClipType.PDF => $"https://www.selaheltelmeez.com/Media21-22/{CurriculumId}/FlipHTML/{fileName}",
                ClipType.YouTube => $"https://www.youtube.com/watch?v={fileName}",
                ClipType.Interactive => $"https://www.selaheltelmeez.com/Media21-22/{CurriculumId}/Interactive/{fileName}",
                ClipType.Exercise => $"https://www.selaheltelmeez.com/Media21-22/{CurriculumId}/Exercise/{fileName}",
                ClipType.Quiz => $"https://www.selaheltelmeez.com/Media21-22/{CurriculumId}/Quiz/",
                ClipType.Dictation => $"https://www.selaheltelmeez.com/Media21-22/{CurriculumId}/Dictation/{fileName}",
                _ => string.Empty,
            };
        }
        private static string getThumbnailUrl(ClipType? clipType, string CurriculumId, int clipId)
        {
            return clipType switch
            {
                ClipType.PDF => $"https://www.selaheltelmeez.com/Media21-22/images/Pages.png",
                ClipType.YouTube => $"https://www.selaheltelmeez.com/Media21-22/{CurriculumId}/LOThumbnail/{clipId}.png",
                ClipType.Interactive => $"https://www.selaheltelmeez.com/Media21-22/{CurriculumId}/LOThumbnail/{clipId}.png",
                ClipType.Exercise => $"https://www.selaheltelmeez.com/Media21-22/images/ExerciseThum.png",
                ClipType.Quiz => $"https://www.selaheltelmeez.com/Media21-22/images/QuizThum.png",
                ClipType.Dictation => $"https://www.selaheltelmeez.com/Media21-22/images/DictationThum.png",
                _ => string.Empty,
            };
        }
    }
}
