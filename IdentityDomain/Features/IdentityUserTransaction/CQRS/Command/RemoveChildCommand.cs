using IdentityDomain.Features.IdentityUserTransaction.DTO;
using ResultHandler;

namespace IdentityDomain.Features.IdentityUserTransaction.CQRS.Command;
public record RemoveChildCommand(RemoveChildRequest RemoveChildRequest) : IRequest<CommitResult>;

