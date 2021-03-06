using Flaminco.CommitResult;
using NotifierDomain.Features.CQRS.DTO.Query;

namespace NotifierDomain.Features.CQRS.Query;
public record GetInvitationsQuery() : IRequest<ICommitResults<InvitationResponse>>;


