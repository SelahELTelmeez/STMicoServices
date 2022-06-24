using TeacherDomain.Features.Quiz.DTO.Command;

namespace TeacherDomain.Features.Quiz.CQRS.Command;
public record ReplyQuizCommand(ReplyQuizRequest ReplyQuizRequest) : IRequest<ICommitResult>;

