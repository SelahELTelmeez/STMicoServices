using DashboardDomain.Features.CQRS.Command;
using DashboardEntity.Entities;
using Flaminco.CommitResult;
using Flaminco.JsonLocalizer;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DomainEntities = DashboardEntity.Entities;

namespace DashboardInfrastructure.Features.Section.CQRS.Command;

public class UpdateSectionCommandHandler : IRequestHandler<UpdateSectionCommand, ICommitResult>
{
    private readonly DashboardDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public UpdateSectionCommandHandler(DashboardDbContext dbContext,
                                       IWebHostEnvironment configuration,
                                       IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(httpContextAccessor, configuration);
    }
    public async Task<ICommitResult> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.Section? section = await _dbContext.Set<DomainEntities.Section>().SingleOrDefaultAsync(a => a.Id.Equals(request.UpdateSectionRequest.Id), cancellationToken);

        if (section == null)
        {
            return ResultType.Duplicated.GetCommitResult("XDAS0003", _resourceJsonManager["XDAS0003"]);
        }
        else
        {
            section.Name = request.UpdateSectionRequest.Name;
            section.Description = request.UpdateSectionRequest.Description;
            section.Thumbnail = request.UpdateSectionRequest.Thumbnail;
            section.SectionGroupId = request.UpdateSectionRequest.SectionGroup;
            section.Type = request.UpdateSectionRequest.Type;
            section.Url = request.UpdateSectionRequest.Url;

            _dbContext.Set<DomainEntities.Section>().Update(section);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetCommitResult();
        }
    }
}