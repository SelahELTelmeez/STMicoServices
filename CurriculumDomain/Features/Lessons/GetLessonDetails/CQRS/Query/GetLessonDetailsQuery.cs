using CurriculumDomain.Features.Lessons.GetLessonDetails.DTO.Query;

namespace CurriculumDomain.Features.Lessons.GetLessonDetails.CQRS.Query;
public record GetLessonDetailsQuery(int LessonId) : IRequest<CommitResult<LessonDetailsReponse>>;


