using TransactionDomain.Features.Quiz.DTO;

namespace TransactionDomain.Features.Quiz.CQRS.Command;

public record CreateQuizCommand(CreateQuizRequest CreateQuizRequest) : IRequest<CommitResult>;


