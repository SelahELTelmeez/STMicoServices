using TransactionDomain.Features.Assignment.DTO;

namespace TransactionDomain.Features.Assignment.CQRS.Command;

public record ReplyAssignmentCommand(ReplyAssignmentRequest ReplyAssignmentRequest) : IRequest<CommitResult>;

