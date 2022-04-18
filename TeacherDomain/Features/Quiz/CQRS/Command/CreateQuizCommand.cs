using TeacherDomain.Features.Quiz.DTO;

namespace TeacherDomain.Features.Quiz.CQRS.Command;
public record CreateQuizCommand(CreateQuizRequest CreateQuizRequest) : IRequest<CommitResult>;


