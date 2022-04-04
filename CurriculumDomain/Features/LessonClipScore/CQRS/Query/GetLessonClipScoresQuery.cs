using CurriculumDomain.Features.LessonClipScore.DTO.Query;
using ResultHandler;

namespace CurriculumDomain.Features.LessonClipScore.CQRS.Query;
public record GetLessonClipScoresQuery(int LessonId) : IRequest<CommitResult<List<LessonClipResponse>>>;