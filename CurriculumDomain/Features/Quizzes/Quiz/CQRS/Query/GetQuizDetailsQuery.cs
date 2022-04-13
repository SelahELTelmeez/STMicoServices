using CurriculumDomain.Features.Quizzes.Quiz.DTO.Query;

namespace CurriculumDomain.Features.Quizzes.Quiz.CQRS.Query;
public record GetQuizDetailsQuery(int QuizId) : IRequest<CommitResult<QuizDetailsResponse>>;