using SharedModule.DTO;

namespace StudentDomain.Features.Tracker.CQRS.Query;

public record GetStudentQuizzResultQuery(string StudentId, int QuizId) : IRequest<ICommitResult<StudentQuizResultResponse>>;

