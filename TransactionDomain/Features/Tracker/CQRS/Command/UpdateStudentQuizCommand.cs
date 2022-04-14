using TransactionDomain.Features.Tracker.DTO.Command;

namespace TransactionDomain.Features.Tracker.CQRS.Command;

public record UpdateStudentQuizCommand(UpdateStudentQuizRequest UpdateStudentQuizRequest) : IRequest<CommitResult>;


