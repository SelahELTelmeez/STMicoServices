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

namespace DashboardInfrastructure.Features.AppSetting.CQRS.Command;

public class InsertAppSettingCommandHandler : IRequestHandler<InsertAppSettingCommand, ICommitResult>
{
    private readonly DashboardDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public InsertAppSettingCommandHandler(DashboardDbContext dbContext, IWebHostEnvironment configuration,
                                          IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(httpContextAccessor, configuration);
    }
    public async Task<ICommitResult> Handle(InsertAppSettingCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.AppSetting? appSetting = await _dbContext.Set<DomainEntities.AppSetting>().FirstOrDefaultAsync(a => a.Name.Equals(request.InsertAppSettingRequest.Name) &&
                                                                                                                            a.Value.Equals(request.InsertAppSettingRequest.Value), cancellationToken);
        if (appSetting == null)
        {
            _dbContext.Set<DomainEntities.AppSetting>().Add(request.InsertAppSettingRequest.Adapt<DomainEntities.AppSetting>());
            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetCommitResult();
        }
        else
        {
            return ResultType.Duplicated.GetCommitResult("XDAS0002", _resourceJsonManager["XDAS0002"]);
        }
    }
}