using Flaminco.CommitResult;
using IdentityDomain.Features.Parent.DTO;

namespace IdentityInfrastructure.Features.Parent.CQRS.Command;

public record AddChildAccountCommand(AddNewChildRequest AddNewChildRequest) : IRequest<ICommitResult>;
