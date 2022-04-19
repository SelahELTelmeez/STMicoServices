using IdentityDomain.Features.IdentityUserTransaction.DTO;
using ResultHandler;

namespace IdentityDomain.Features.IdentityUserTransaction.CQRS.Command;

public record AddNewChildCommand(AddNewChildRequest AddNewChildRequest) : IRequest<CommitResult<AddNewChildResponse>>;