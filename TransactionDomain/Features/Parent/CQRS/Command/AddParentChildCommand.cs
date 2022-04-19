using TransactionDomain.Features.Parent.DTO;

namespace TransactionDomain.Features.Parent.CQRS.Command;

public record AddParentChildCommand(AddParentChildRequest AddParentChildRequest) : IRequest<CommitResult<AddParentChildResponse>>;
