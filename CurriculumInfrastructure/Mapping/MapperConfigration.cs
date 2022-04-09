using CurriculumDomain.Features.Lessons.GetLessonClips.DTO.Query;
using CurriculumDomain.Features.Subjects.GetStudentSubjects.DTO.Query;
using CurriculumDomain.Features.Subjects.GetSubjectBrief.DTO.Query;
using CurriculumDomain.Features.Subjects.GetSubjectUnits.DTO.Query;
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

            TypeAdapterConfig<Domain.Subjects.Subject, IdnentitySubjectResponse>.NewConfig()
              .Map(dis => dis.Name, src => src.ShortName)
              .Map(dis => dis.BackgroundImage, src => $"https://www.selaheltelmeez.com/Media21-22/LMSAPP/SubjectIcon/slider/{src.Title}.png")
              .Map(dis => dis.Icon, src => $"https://www.selaheltelmeez.com/Media21-22/LMSAPP/SubjectIcon/coloredIcon/{src.Title}.png");

            TypeAdapterConfig<Domain.Units.Unit, UnitResponse>.NewConfig()
              .Map(dis => dis.Name, src => src.ShortName); /// can not map short name in unit to be name;

            TypeAdapterConfig<Domain.Lessons.Lesson, LessonResponse>.NewConfig()
              .Map(dis => dis.Name, src => src.ShortName);

            TypeAdapterConfig<Domain.Subjects.Subject, SubjectBriefResponse>.NewConfig()
             .Map(dis => dis.Id, src => src.Id)
             .Map(dis => dis.Name, src => src.ShortName);

            TypeAdapterConfig<Domain.Clips.Clip, ClipResponse>.NewConfig()
              .Map(dis => dis.Id, src => src.Id)
              .Map(dis => dis.ClipName, src => src.Title)
              .Map(dis => dis.ClipType, src => src.Type)
              .Map(dis => dis.LessonId, src => src.LessonId.GetValueOrDefault())
              .Map(dis => dis.ClipScore, src => src.Points)
              .Map(dis => dis.IsPremiumOnly, src => src.IsPremium.GetValueOrDefault())
              .Map(dis => dis.GameObjectUrl, src => getGameObjectUrl(src.Type, src.LessonFK.UnitFK.SubjectFK.Id, src.FileName))
              .Map(dis => dis.Thumbnail, src => getThumbnailUrl(src.Type, src.LessonFK.UnitFK.SubjectFK.Id, src.Id));


            TypeAdapterConfig<(Domain.Clips.Clip Clip, ClipActivityResponse ClipActivity), ClipResponse>.NewConfig()
                   .Map(dis => dis.Id, src => src.Clip.Id)
                   .Map(dis => dis.Width, src => src.Clip.Width)
                   .Map(dis => dis.Height, src => src.Clip.Height)
                   .Map(dis => dis.Orientation, src => src.Clip.Orientation)
                  .Map(dis => dis.ClipName, src => src.Clip.Title)
                  .Map(dis => dis.ClipType, src => src.Clip.Type)
                  .Map(dis => dis.LessonId, src => src.Clip.LessonId.GetValueOrDefault())
                  .Map(dis => dis.ClipScore, src => src.Clip.Points)
                  .Map(dis => dis.IsPremiumOnly, src => src.Clip.IsPremium.GetValueOrDefault())
                  .Map(dis => dis.GameObjectUrl, src => getGameObjectUrl(src.Clip.Type, src.Clip.LessonFK.UnitFK.SubjectFK.Id, src.Clip.FileName))
                  .Map(dis => dis.Thumbnail, src => getThumbnailUrl(src.Clip.Type, src.Clip.LessonFK.UnitFK.SubjectFK.Id, src.Clip.Id))
                  .Map(dis => dis.ActivityId, src => src.ClipActivity.ActivityId)
                  .Map(dis => dis.StudentScore, src => src.ClipActivity.StudentScore)
                  .Map(dis => dis.GameObjectCode, src => src.ClipActivity.GameObjectCode)
                  .Map(dis => dis.GameObjectLearningDurationInSec, src => src.ClipActivity.GameObjectLearningDurationInSec)
                  .Map(dis => dis.GameObjectProgress, src => src.ClipActivity.GameObjectProgress);

            TypeAdapterConfig<Domain.Clips.Clip, FilterTypesResponse>.NewConfig()
               .Map(dis => dis.ImageUrl, src => $"https://www.selaheltelmeez.com/Media21-22/LMSApp/FilterImage/{(int)src.Type}.png")
               .Map(dis => dis.Name, src => Enum.GetName(typeof(ClipType), src.Type.GetValueOrDefault()))
               .Map(dis => dis.Value, src => src.Type);


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