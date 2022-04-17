using TransactionDomain.Features.Quiz.DTO;

namespace TransactionDomain.Features.Quiz.CQRS.Command;

public record ReplyQuizCommand(ReplyQuizRequest ReplyQuizRequest) : IRequest<CommitResult>;

