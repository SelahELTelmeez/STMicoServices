using DashboardDomain.Features.CQRS.Command;
using DashboardEntity.Entities;
using Flaminco.CommitResult;
using Flaminco.JsonLocalizer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainEntities = DashboardEntity.Entities;

namespace DashboardInfrastructure.Features.Section.CQRS.Command;

public class UpdateSectionCommandHandler : IRequestHandler<UpdateSectionCommand, ICommitResult>
{
    private readonly DashboardDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public UpdateSectionCommandHandler(DashboardDbContext dbContext, JsonLocalizerManager jsonLocalizerManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = jsonLocalizerManager;
    }
    public async Task<ICommitResult> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.Section? section = await _dbContext.Set<DomainEntities.Section>().SingleOrDefaultAsync(a => a.Name.Equals(request.UpdateSectionRequest.Id), cancellationToken);

        if (section == null)
        {
            return ResultType.Duplicated.GetCommitResult("X0004", _resourceJsonManager["X0004"]);
        }
        else
        {
            section.Name = request.UpdateSectionRequest.Name;
            section.Description = request.UpdateSectionRequest.Description;
            section.Thumbnail = request.UpdateSectionRequest.Thumbnail;
            section.SectionGroupId = request.UpdateSectionRequest.SectionGroup;
            section.Type = request.UpdateSectionRequest.Type;
            section.YouTubeId = request.UpdateSectionRequest.YouTubeId;

            _dbContext.Set<DomainEntities.Section>().Update(section);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetCommitResult();
        }
    }
}