using DashboardDomain.Features.CQRS.Command;
using DashboardEntity.Entities;
using Flaminco.CommitResult;
using Flaminco.JsonLocalizer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainEntities = DashboardEntity.Entities;

namespace DashboardInfrastructure.Features.SectionGroup.CQRS.Command;

public class InsertSectionGroupCommandHandler : IRequestHandler<InsertSectionGroupCommand, ICommitResult>
{
    private readonly DashboardDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public InsertSectionGroupCommandHandler(DashboardDbContext dbContext, JsonLocalizerManager jsonLocalizerManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = jsonLocalizerManager;
    }
    public async Task<ICommitResult> Handle(InsertSectionGroupCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.SectionGroup? sectionGroup = await _dbContext.Set<DomainEntities.SectionGroup>().SingleOrDefaultAsync(a => a.Name.Equals(request.Name), cancellationToken);

        if (sectionGroup == null)
        {
            await _dbContext.Set<DomainEntities.SectionGroup>().AddAsync(new DomainEntities.SectionGroup { Name = request.Name });
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ResultType.Ok.GetCommitResult();
        }
        else
        {
            return ResultType.Duplicated.GetCommitResult("X0004", _resourceJsonManager["X0004"]);
        }
    }
}