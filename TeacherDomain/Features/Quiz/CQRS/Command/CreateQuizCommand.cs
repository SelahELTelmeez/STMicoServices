using TeacherDomain.Features.Quiz.Command.DTO;

namespace TeacherDomain.Features.Quiz.CQRS.Command;
public record CreateQuizCommand(CreateQuizRequest CreateQuizRequest) : IRequest<ICommitResult>;


