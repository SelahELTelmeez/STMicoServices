using Mapster;
using TransactionDomain.Features.Invitations.CQRS.Command;
using TransactionEntites.Entities;
using DomainEntities = TransactionEntites.Entities.Invitation;

namespace TransactionInfrastructure.Features.Invitations.CQRS.Command;
public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, CommitResult>
{
    private readonly TrackerDbContext _dbContext;
    public CreateInvitationCommandHandler(TrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<CommitResult> Handle(CreateInvitationCommand request, CancellationToken cancellationToken)
    {
        _dbContext.Set<DomainEntities.Invitation>().Add(request.InvitationRequest.Adapt<DomainEntities.Invitation>());
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}
