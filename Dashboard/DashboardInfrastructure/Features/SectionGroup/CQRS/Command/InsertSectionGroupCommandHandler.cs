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

public class InsertSectionGroupCommandHandler : IRequestHandler<InsertSectionGroupCommand, ICommitResult>
{
    private readonly DashboardDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public InsertSectionGroupCommandHandler(DashboardDbContext dbContext,
                                            IWebHostEnvironment configuration,
                                            IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(httpContextAccessor, configuration);
    }
    public async Task<ICommitResult> Handle(InsertSectionGroupCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.SectionGroup? sectionGroup = await _dbContext.Set<DomainEntities.SectionGroup>().FirstOrDefaultAsync(a => a.Name.Equals(request.Name), cancellationToken);

        if (sectionGroup == null)
        {
            _dbContext.Set<DomainEntities.SectionGroup>().Add(new DomainEntities.SectionGroup { Name = request.Name });

            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetCommitResult();
        }
        else
        {
            return ResultType.Duplicated.GetCommitResult("XDAS0006", _resourceJsonManager["XDAS0006"]);
        }
    }
}