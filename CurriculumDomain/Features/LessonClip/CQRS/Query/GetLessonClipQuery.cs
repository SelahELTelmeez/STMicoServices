using CurriculumDomain.Features.LessonClip.DTO.Query;
using ResultHandler;

namespace CurriculumDomain.Features.LessonClip.CQRS.Query;
public record GetLessonClipQuery(int LessonId) : IRequest<CommitResult<LessonClipResponse>>;