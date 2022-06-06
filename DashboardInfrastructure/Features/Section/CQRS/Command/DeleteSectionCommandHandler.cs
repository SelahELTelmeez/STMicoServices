using DashboardDomain.Features.CQRS.Command;
using DashboardEntity.Entities;
using Flaminco.CommitResult;
using Flaminco.JsonLocalizer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainEntities = DashboardEntity.Entities;

namespace DashboardInfrastructure.Features.Section.CQRS.Command;

public class DeleteSectionCommandHandler : IRequestHandler<DeleteSectionCommand, ICommitResult>
{
    private readonly DashboardDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public DeleteSectionCommandHandler(DashboardDbContext dbContext, JsonLocalizerManager jsonLocalizerManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = jsonLocalizerManager;
    }
    public async Task<ICommitResult> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.Section? section = await _dbContext.Set<DomainEntities.Section>().SingleOrDefaultAsync(a => a.Id.Equals(request.Id), cancellationToken);

        if (section == null)
        {
            return ResultType.NotFound.GetCommitResult("X0004", _resourceJsonManager["X0004"]);
        }
        else
        {
            _dbContext.Set<DomainEntities.Section>().Remove(section);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetCommitResult();
        }
    }
}