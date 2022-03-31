using CurriculumDomain.Features.StudentRecentLessonsProgress.DTO.Query;
using ResultHandler;

namespace CurriculumDomain.Features.LessonDetails.CQRS.Query;

public record GetLessonDetailsByIdQuery(int LessonId) : IRequest<CommitResult<LessonDetailsReponse>>;


