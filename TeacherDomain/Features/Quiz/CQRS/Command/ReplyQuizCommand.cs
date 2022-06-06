using TeacherDomain.Features.Quiz.Command.DTO;

namespace TeacherDomain.Features.Quiz.CQRS.Command;
public record ReplyQuizCommand(ReplyQuizRequest ReplyQuizRequest) : IRequest<ICommitResult>;

