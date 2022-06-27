using DashboardDomain.Features.CQRS.Command;
using DashboardEntity.Entities;
using Flaminco.CommitResult;
using Flaminco.JsonLocalizer;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DomainEntities = DashboardEntity.Entities;

namespace DashboardInfrastructure.Features.Section.CQRS.Command;

public class InsertSectionCommandHandler : IRequestHandler<InsertSectionCommand, ICommitResult>
{
    private readonly DashboardDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public InsertSectionCommandHandler(DashboardDbContext dbContext,
                                       IWebHostEnvironment configuration,
                                       IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(httpContextAccessor, configuration);
    }
    public async Task<ICommitResult> Handle(InsertSectionCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.Section? section = await _dbContext.Set<DomainEntities.Section>().SingleOrDefaultAsync(a => a.Name.Equals(request.InsertSectionRequest.Name) &&
                                                                                                              a.Type.Equals(request.InsertSectionRequest.Type), cancellationToken);
        if (section == null)
        {
            _dbContext.Set<DomainEntities.Section>().Add(request.InsertSectionRequest.Adapt<DomainEntities.Section>());
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ResultType.Ok.GetCommitResult();
        }
        else
        {
            return ResultType.Duplicated.GetCommitResult("XDAS0004", _resourceJsonManager["XDAS0004"]);

        }
    }
}