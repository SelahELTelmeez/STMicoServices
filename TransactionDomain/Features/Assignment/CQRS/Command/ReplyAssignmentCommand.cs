using TransactionDomain.Features.Assignment.DTO.Command;

namespace TransactionDomain.Features.Assignment.CQRS.Command;

public record ReplyAssignmentCommand(ReplyAssignmentRequest ReplyAssignmentRequest) : IRequest<CommitResult>;

