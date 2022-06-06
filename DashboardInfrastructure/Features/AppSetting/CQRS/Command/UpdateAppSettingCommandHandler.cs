using DashboardDomain.Features.CQRS.Command;
using DashboardEntity.Entities;
using Flaminco.CommitResult;
using Flaminco.JsonLocalizer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainEntities = DashboardEntity.Entities;

namespace DashboardInfrastructure.Features.AppSetting.CQRS.Command;

public class UpdateAppSettingCommandHandler : IRequestHandler<UpdateAppSettingCommand, ICommitResult>
{
    private readonly DashboardDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public UpdateAppSettingCommandHandler(DashboardDbContext dbContext, JsonLocalizerManager jsonLocalizerManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = jsonLocalizerManager;
    }
    public async Task<ICommitResult> Handle(UpdateAppSettingCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.AppSetting? appSetting = await _dbContext.Set<DomainEntities.AppSetting>().SingleOrDefaultAsync(a => a.Name.Equals(request.UpdateAppSettingRequest.Id), cancellationToken);

        if (appSetting == null)
        {
            return ResultType.Duplicated.GetCommitResult("X0004", _resourceJsonManager["X0004"]);
        }
        else
        {
            appSetting.Name = request.UpdateAppSettingRequest.Name;
            appSetting.Value = request.UpdateAppSettingRequest.Value;
            appSetting.IsEnabled = request.UpdateAppSettingRequest.IsEnabled;

            _dbContext.Set<DomainEntities.AppSetting>().Update(appSetting);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetCommitResult();
        }
    }
}