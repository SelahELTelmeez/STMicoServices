using CurriculumDomain.Features.Quizzes.DTO.Query;

namespace CurriculumDomain.Features.Quizzes.CQRS.Query;

public record GetStudentAttemptsQuery(string StudentId, int QuizId) : IRequest<CommitResult<QuizAttemptResponse>>;


