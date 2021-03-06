using CurriculumDomain.Features.Lessons.GetLessonClips.DTO.Query;
using CurriculumDomain.Features.Quizzes.DTO.Query;
using CurriculumDomain.Features.Subjects.GetStudentSubjects.DTO.Query;
using CurriculumEntites.Entities.MCQS;
using CurriculumEntites.Entities.Quizzes;
using CurriculumEntites.Entities.Shared;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using SharedModule.DTO;
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
              .Map(dis => dis.Icon, src => $"https://www.selaheltelmeez.com/Media21-22/LMSAPP/SubjectIcon/coloredIcon/{src.Title}.png")
              .Map(dis => dis.TeacherGuide, src => $"https://www.selaheltelmeez.com/media2021/{src.TeacherGuide}");

            TypeAdapterConfig<Domain.Units.Unit, UnitResponse>.NewConfig()
              .Map(dis => dis.Name, src => src.ShortName)
              .Map(dis => dis.SubjectId, src => src.SubjectId); /// can not map short name in unit to be name;

            TypeAdapterConfig<Domain.Lessons.Lesson, LessonResponse>.NewConfig()
              .Map(dis => dis.Name, src => src.ShortName);

            TypeAdapterConfig<Domain.Subjects.Subject, SubjectBriefResponse>.NewConfig()
             .Map(dis => dis.Id, src => src.Id)
             .Map(dis => dis.BackgroundImage, src => $"https://www.selaheltelmeez.com/Media21-22/LMSAPP/SubjectIcon/slider/{src.Title}.png")
             .Map(dis => dis.InternalIcon, src => "http://www.almoallem.com/media/LMSAPP/SubjectIcon/Icon/teacher/{subject.Title}.png")
             .Map(dis => dis.Name, src => src.ShortName);

            TypeAdapterConfig<Domain.Clips.Clip, ClipResponse>.NewConfig()
              .Map(dis => dis.Id, src => src.Id)
              .Map(dis => dis.ClipName, src => src.Title)
              .Map(dis => dis.ClipType, src => src.Type)
              .Map(dis => dis.LessonId, src => src.LessonId.GetValueOrDefault())
              .Map(dis => dis.ClipScore, src => src.Points)
              .Map(dis => dis.IsPremiumOnly, src => src.IsPremium.GetValueOrDefault(true))
              .Map(dis => dis.GameObjectUrl, src => getGameObjectUrl(src.Type, src.LessonFK.UnitFK.SubjectFK.Id, src.FileName))
              .Map(dis => dis.Thumbnail, src => getThumbnailUrl(src.Type, src.LessonFK.UnitFK.SubjectFK.Id, src.Id));

            TypeAdapterConfig<Domain.Clips.Clip, QuizClipResponse>.NewConfig()
             .Map(dis => dis.Id, src => src.Id)
             .Map(dis => dis.ClipName, src => src.Title)
             .Map(dis => dis.ClipType, src => src.Type)
             .Map(dis => dis.LessonId, src => src.LessonId.GetValueOrDefault())
             .Map(dis => dis.ClipScore, src => src.Points)
             .Map(dis => dis.IsPremiumOnly, src => src.IsPremium.GetValueOrDefault(true))
             .Map(dis => dis.GameObjectUrl, src => getGameObjectUrl(src.Type, src.LessonFK.UnitFK.SubjectId, src.FileName))
             .Map(dis => dis.Thumbnail, src => getThumbnailUrl(src.Type, src.LessonFK.UnitFK.SubjectId, src.Id));


            TypeAdapterConfig<(Domain.Clips.Clip Clip, ClipActivityResponse ClipActivity), ClipResponse>.NewConfig()
                   .Map(dis => dis.Id, src => src.Clip.Id)
                   .Map(dis => dis.Width, src => src.Clip.Width)
                   .Map(dis => dis.Height, src => src.Clip.Height)
                   .Map(dis => dis.Orientation, src => src.Clip.Orientation)
                  .Map(dis => dis.ClipName, src => src.Clip.Title)
                  .Map(dis => dis.ClipType, src => src.Clip.Type)
                  .Map(dis => dis.LessonId, src => src.Clip.LessonId.GetValueOrDefault())
                  .Map(dis => dis.ClipScore, src => src.Clip.Points)
                  .Map(dis => dis.IsPremiumOnly, src => src.Clip.IsPremium.GetValueOrDefault(true))
                  .Map(dis => dis.GameObjectUrl, src => getGameObjectUrl(src.Clip.Type, src.Clip.LessonFK.UnitFK.SubjectFK.Id, src.Clip.FileName))
                  .Map(dis => dis.Thumbnail, src => getThumbnailUrl(src.Clip.Type, src.Clip.LessonFK.UnitFK.SubjectFK.Id, src.Clip.Id))
                  .Map(dis => dis.ActivityId, src => src.ClipActivity.Id)
                  .Map(dis => dis.StudentPoints, src => src.ClipActivity.StudentPoints)
                  .Map(dis => dis.GameObjectCode, src => src.ClipActivity.GameObjectCode)
                  .Map(dis => dis.GameObjectLearningDurationInSec, src => src.ClipActivity.GameObjectLearningDurationInSec)
                  .Map(dis => dis.GameObjectProgress, src => src.ClipActivity.GameObjectProgress);

            TypeAdapterConfig<Domain.Clips.Clip, FilterTypesResponse>.NewConfig()
               .Map(dis => dis.ImageUrl, src => $"https://www.selaheltelmeez.com/Media21-22/LMSApp/FilterImage/{(int)src.Type}.png")
               .Map(dis => dis.Name, src => Enum.GetName(typeof(ClipType), src.Type.GetValueOrDefault()))
               .Map(dis => dis.Value, src => src.Type);


            TypeAdapterConfig<MCQQuestion, QuizQuestion>.NewConfig()
              .Ignore(dis => dis.Id);


            TypeAdapterConfig<MCQAnswer, QuizAnswer>.NewConfig()
              .Ignore(dis => dis.Id)
              .Ignore(dis => dis.QuizFormId);

            TypeAdapterConfig<MCQ, QuizForm>.NewConfig()
               .Map(dis => dis.DurationInSec, src => src.DurationInSec)
               .Map(dis => dis.Hint, src => src.Hint)
               .Map(dis => dis.Question, src => src.Question)
               .Map(dis => dis.Answers, src => src.Answers)
               .Ignore(src => src.Id);


            return services;
        }
        private static string getGameObjectUrl(ClipType? clipType, string SubjectId, string fileName)
        {
            return clipType switch
            {
                ClipType.Interactive => $"https://www.selaheltelmeez.com/Media21-22/{SubjectId}/Interactive/{fileName}",
                ClipType.YouTube => $"https://www.youtube.com/watch?v={fileName}",
                ClipType.Try => $"https://www.selaheltelmeez.com/Media21-22/{SubjectId}/Try/{fileName}",
                ClipType.Game => $"https://www.selaheltelmeez.com/Media21-22/{SubjectId}/Game/{fileName}",
                ClipType.PDF => $"https://www.selaheltelmeez.com/Media21-22/{SubjectId}/FlipHTML/{fileName}",
                ClipType.Quiz => $"https://www.selaheltelmeez.com/Media21-22/{SubjectId}/FlipHTML/{fileName}",
                ClipType.Exam => $"https://www.selaheltelmeez.com/Media21-22/{SubjectId}/Exam/{fileName}",
                ClipType.Exercise => $"https://www.selaheltelmeez.com/Media21-22/{SubjectId}/Exercise/{fileName}",
                ClipType.Dictation => $"https://www.selaheltelmeez.com/Media21-22/{SubjectId}/Dictation/{fileName}",
                _ => string.Empty,
            };
        }
        private static string getThumbnailUrl(ClipType? clipType, string SubjectId, int clipId)
        {
            return clipType switch
            {
                ClipType.PDF => $"https://www.selaheltelmeez.com/Media21-22/images/Pages.png",
                ClipType.YouTube => $"https://www.selaheltelmeez.com/Media21-22/{SubjectId}/LOThumbnail/{clipId}.png",
                ClipType.Quiz => $"https://www.selaheltelmeez.com/Media21-22/images/QuizThum.png",
                ClipType.Exam => $"https://www.selaheltelmeez.com/Media21-22/images/ExamThum.png",
                ClipType.Exercise => $"https://www.selaheltelmeez.com/Media21-22/images/ExerciseThum.png",
                ClipType.ExerciseMCQ => $"https://www.selaheltelmeez.com/Media21-22/images/QuizThum.png",
                ClipType.Interactive => $"https://www.selaheltelmeez.com/Media21-22/{SubjectId}/LOThumbnail/{clipId}.png",
                ClipType.Dictation => $"https://www.selaheltelmeez.com/Media21-22/images/DictationThum.png",
                _ => $"https://www.selaheltelmeez.com/Media21-22/{SubjectId}/LOThumbnail/{clipId}.png",
            };
        }
    }
}