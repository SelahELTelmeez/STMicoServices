using TeacherDomain.Features.Quiz.DTO;

namespace TeacherDomain.Features.Quiz.CQRS.Command;
public record ReplyQuizCommand(ReplyQuizRequest ReplyQuizRequest) : IRequest<CommitResult>;

