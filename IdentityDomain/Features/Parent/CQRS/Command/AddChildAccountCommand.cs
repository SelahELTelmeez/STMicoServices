using IdentityDomain.Features.Parent.DTO;
using ResultHandler;

namespace IdentityInfrastructure.Features.Parent.CQRS.Command;

public record AddChildAccountCommand(AddNewChildRequest AddNewChildRequest) : IRequest<CommitResult>;
