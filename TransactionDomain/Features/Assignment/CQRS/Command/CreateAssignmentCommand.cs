using TransactionDomain.Features.Assignment.DTO.Command;

namespace TransactionDomain.Features.Assignment.CQRS.Command;

public record CreateAssignmentCommand(CreateAssignmentRequest CreateAssignmentRequest) : IRequest<CommitResult>;