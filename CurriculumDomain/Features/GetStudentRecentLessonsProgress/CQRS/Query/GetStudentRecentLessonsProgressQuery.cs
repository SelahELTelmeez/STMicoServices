using CurriculumDomain.Features.GetStudentRecentLessonsProgress.DTO;
using ResultHandler;

namespace CurriculumDomain.Features.GetStudentRecentLessonsProgress.CQRS.Query;

public record GetStudentRecentLessonsProgressQuery : IRequest<CommitResult<List<StudentRecentLessonProgress>>>;