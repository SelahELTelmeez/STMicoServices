namespace CurriculumDomain.Features.Quizzes.CQRS.Command;
public record CreateQuizCommand(int ClipId) : IRequest<CommitResult<int>>;