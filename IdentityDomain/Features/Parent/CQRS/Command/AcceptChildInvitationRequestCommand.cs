using Flaminco.CommitResult;
using IdentityDomain.Features.Parent.DTO;

namespace IdentityDomain.Features.Parent.CQRS.Command;

public record AcceptChildInvitationRequestCommand(AddChildInvitationRequest AddChildInvitationRequest) : IRequest<ICommitResult>;


