using MediatR;
using ResultHandler;
using TransactionDomain.Features.Tracker.DTO;

namespace TransactionDomain.Features.Tracker.CQRS.Query;

public record GetStudentRecentLessonsProgressQuery : IRequest<CommitResults<StudentRecentLessonProgressResponse>>;