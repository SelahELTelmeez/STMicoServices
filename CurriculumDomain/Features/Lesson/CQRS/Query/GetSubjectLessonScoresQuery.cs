using CurriculumDomain.Features.Lesson.DTO.Query;
using ResultHandler;

namespace CurriculumDomain.Features.Lesson.CQRS.Query;
public record GetSubjectLessonScoresQuery(string SubjectId) : IRequest<CommitResult<List<LessonResponse>>>;