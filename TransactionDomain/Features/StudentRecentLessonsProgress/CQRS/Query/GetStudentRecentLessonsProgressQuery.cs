using MediatR;
using ResultHandler;
using TransactionDomain.Features.GetStudentRecentLessonsProgress.DTO;

namespace TransactionDomain.Features.StudentRecentLessonsProgress.CQRS.Query;

public record GetStudentRecentLessonsProgressQuery : IRequest<CommitResult<List<StudentRecentLessonProgressResponse>>>;