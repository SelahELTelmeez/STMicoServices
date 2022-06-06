using DashboardDomain.Features.CQRS.Command;
using DashboardEntity.Entities;
using Flaminco.CommitResult;
using Flaminco.JsonLocalizer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainEntities = DashboardEntity.Entities;

namespace DashboardInfrastructure.Features.SectionGroup.CQRS.Command;

public class UpdateSectionGroupCommandHandler : IRequestHandler<UpdateSectionGroupCommand, ICommitResult>
{
    private readonly DashboardDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public UpdateSectionGroupCommandHandler(DashboardDbContext dbContext, JsonLocalizerManager jsonLocalizerManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = jsonLocalizerManager;
    }
    public async Task<ICommitResult> Handle(UpdateSectionGroupCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.SectionGroup? sectionGroup = await _dbContext.Set<DomainEntities.SectionGroup>().SingleOrDefaultAsync(a => a.Name.Equals(request.Id), cancellationToken);

        if (sectionGroup == null)
        {
            return ResultType.Duplicated.GetCommitResult("X0004", _resourceJsonManager["X0004"]);
        }
        else
        {
            sectionGroup.Name = request.Name;

            _dbContext.Set<DomainEntities.SectionGroup>().Update(sectionGroup);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetCommitResult();
        }
    }
}