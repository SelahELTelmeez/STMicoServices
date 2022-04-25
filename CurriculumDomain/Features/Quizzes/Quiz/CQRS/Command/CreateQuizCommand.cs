namespace CurriculumDomain.Features.Quizzes.Quiz.CQRS.Command;
public record CreateQuizCommand(int ClipId) : IRequest<CommitResult<int>>;