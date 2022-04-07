using CurriculumDomain.Features.Lessons.GetLessonClips.DTO.Query;

namespace CurriculumDomain.Features.Lessons.GetLessonClips.CQRS.Query;
public record GetLessonClipQuery(int LessonId) : IRequest<CommitResult<LessonClipResponse>>;