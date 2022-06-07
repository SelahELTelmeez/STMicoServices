using IdentityDomain.Features.Parent.DTO;
using ResultHandler;

namespace IdentityDomain.Features.Parent.CQRS.Command;

public record AcceptChildInvitationRequestCommand(AddChildInvitationRequest AddChildInvitationRequest) : IRequest<CommitResult>;


