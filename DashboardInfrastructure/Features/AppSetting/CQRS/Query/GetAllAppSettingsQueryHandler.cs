using DashboardDomain.Features.CQRS.Query;
using DashboardDomain.Features.DTO.Query;
using DashboardEntity.Entities;
using Flaminco.CommitResult;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainEntities = DashboardEntity.Entities;

namespace DashboardInfrastructure.Features.AppSetting.CQRS.Query;

public class GetAllAppSettingsQueryHandler : IRequestHandler<GetAllAppSettingsQuery, ICommitResults<AppSettingResponse>>
{
    private readonly DashboardDbContext _dbContext;

    public GetAllAppSettingsQueryHandler(DashboardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ICommitResults<AppSettingResponse>> Handle(GetAllAppSettingsQuery request, CancellationToken cancellationToken)
    {
        return ResultType.Ok.GetValueCommitResults(await _dbContext.Set<DomainEntities.AppSetting>().Select(a => new AppSettingResponse { Name = a.Name , Value = a.Value  }).ToListAsync(cancellationToken));
    }
}