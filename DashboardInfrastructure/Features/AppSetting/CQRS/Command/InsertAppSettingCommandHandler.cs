using DashboardDomain.Features.CQRS.Command;
using DashboardEntity.Entities;
using Flaminco.CommitResult;
using Flaminco.JsonLocalizer;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainEntities = DashboardEntity.Entities;

namespace DashboardInfrastructure.Features.AppSetting.CQRS.Command;

public class InsertAppSettingCommandHandler : IRequestHandler<InsertAppSettingCommand, ICommitResult>
{
    private readonly DashboardDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public InsertAppSettingCommandHandler(DashboardDbContext dbContext, JsonLocalizerManager jsonLocalizerManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = jsonLocalizerManager;
    }
    public async Task<ICommitResult> Handle(InsertAppSettingCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.AppSetting? appSetting = await _dbContext.Set<DomainEntities.AppSetting>().SingleOrDefaultAsync(a => a.Name.Equals(request.InsertAppSettingRequest.Name) && 
                                                                                                                            a.Value.Equals(request.InsertAppSettingRequest.Value), cancellationToken);

        if (appSetting == null)
        {
            return ResultType.NotFound.GetCommitResult("X0004", _resourceJsonManager["X0004"]);
        }
        else
        {
            await _dbContext.Set<DomainEntities.AppSetting>().AddAsync(request.InsertAppSettingRequest.Adapt<DomainEntities.AppSetting>());
            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetCommitResult();
        }
    }
}