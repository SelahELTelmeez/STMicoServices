using Mapster;
using MediatR;
using ResultHandler;
using TransactionDomain.Features.Invitation.CQRS.Command;
using TransactionEntites.Entities;
using DomainEntities = TransactionEntites.Entities.Invitation;

namespace TransactionInfrastructure.Features.Invitation.CQRS;

public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, CommitResult>
{
    private readonly StudentTrackerDbContext _dbContext;
    public CreateInvitationCommandHandler(StudentTrackerDbContext dbContext)
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
