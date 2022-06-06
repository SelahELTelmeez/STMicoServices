using DashboardDomain.Features.CQRS.Command;
using Flaminco.CommitResult;
using DomainEntities = DashboardEntity.Entities;
using MediatR;
using DashboardEntity.Entities;
using Microsoft.EntityFrameworkCore;
using Flaminco.JsonLocalizer;

namespace DashboardInfrastructure.Features.AppSetting.CQRS.Command;

public class DeleteAppSettingCommandHandler : IRequestHandler<DeleteAppSettingCommand, ICommitResult>
{
    private readonly DashboardDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public DeleteAppSettingCommandHandler(DashboardDbContext dbContext, JsonLocalizerManager jsonLocalizerManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = jsonLocalizerManager;
    }
    public async Task<ICommitResult> Handle(DeleteAppSettingCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.AppSetting? appSetting = await _dbContext.Set<DomainEntities.AppSetting>().SingleOrDefaultAsync(a => a.Id.Equals(request.Id), cancellationToken);

        if (appSetting == null)
        {
            return ResultType.NotFound.GetCommitResult("X0004", _resourceJsonManager["X0004"]);
        }
        else
        {
            //_dbContext.Set<DomainEntities.AppSetting>().Remove(appSetting);
            appSetting.IsEnabled = !appSetting.IsEnabled;

            _dbContext.Set<DomainEntities.AppSetting>().Update(appSetting);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetCommitResult();
        }
    }
}