using CurriculumDomain.Features.Quizzes.DTO.Query;

namespace CurriculumDomain.Features.Quizzes.CQRS.Query;
public record GetQuizDetailsQuery(int QuizId) : IRequest<CommitResult<QuizDetailsResponse>>;