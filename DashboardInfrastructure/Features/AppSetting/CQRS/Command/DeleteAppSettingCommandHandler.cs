using DashboardDomain.Features.CQRS.Command;
using DashboardEntity.Entities;
using Flaminco.CommitResult;
using Flaminco.JsonLocalizer;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DomainEntities = DashboardEntity.Entities;

namespace DashboardInfrastructure.Features.AppSetting.CQRS.Command;

public class DeleteAppSettingCommandHandler : IRequestHandler<DeleteAppSettingCommand, ICommitResult>
{
    private readonly DashboardDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public DeleteAppSettingCommandHandler(DashboardDbContext dbContext,
                                          IWebHostEnvironment configuration,
                                          IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(httpContextAccessor, configuration);
    }
    public async Task<ICommitResult> Handle(DeleteAppSettingCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.AppSetting? appSetting = await _dbContext.Set<DomainEntities.AppSetting>().SingleOrDefaultAsync(a => a.Id.Equals(request.Id), cancellationToken);

        if (appSetting == null)
        {
            return ResultType.NotFound.GetCommitResult("XDAS0001", _resourceJsonManager["XDAS0001"]);
        }
        else
        {
            _dbContext.Set<DomainEntities.AppSetting>().Remove(appSetting);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetCommitResult();
        }
    }
}