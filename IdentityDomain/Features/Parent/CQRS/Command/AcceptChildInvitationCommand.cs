using IdentityDomain.Features.Parent.DTO;
using ResultHandler;

namespace IdentityDomain.Features.Parent.CQRS.Command;

public record AcceptChildInvitationCommand(AddChildInvitationRequest AddChildInvitationRequest) : IRequest<CommitResult>;


