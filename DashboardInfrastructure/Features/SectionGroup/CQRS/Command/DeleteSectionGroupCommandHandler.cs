using DashboardDomain.Features.CQRS.Command;
using DashboardEntity.Entities;
using Flaminco.CommitResult;
using Flaminco.JsonLocalizer;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DomainEntities = DashboardEntity.Entities;

namespace DashboardInfrastructure.Features.SectionGroup.CQRS.Command;

public class DeleteSectionGroupCommandHandler : IRequestHandler<DeleteSectionGroupCommand, ICommitResult>
{
    private readonly DashboardDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public DeleteSectionGroupCommandHandler(DashboardDbContext dbContext,
                                            IWebHostEnvironment configuration,
                                            IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(httpContextAccessor, configuration);
    }
    public async Task<ICommitResult> Handle(DeleteSectionGroupCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.SectionGroup? sectionGroup = await _dbContext.Set<DomainEntities.SectionGroup>().FirstOrDefaultAsync(a => a.Id.Equals(request.Id), cancellationToken);

        if (sectionGroup == null)
        {
            return ResultType.NotFound.GetCommitResult("XDAS0005", _resourceJsonManager["XDAS0005"]);
        }
        else
        {
            _dbContext.Set<DomainEntities.SectionGroup>().Remove(sectionGroup);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetCommitResult();
        }
    }
}