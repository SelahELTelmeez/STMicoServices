using SharedModule.DTO;

namespace NotifierDomain.Features.CQRS.Command;
public record SendInvitationCommand(InvitationRequest InvitationRequest) : IRequest<ICommitResult>;

