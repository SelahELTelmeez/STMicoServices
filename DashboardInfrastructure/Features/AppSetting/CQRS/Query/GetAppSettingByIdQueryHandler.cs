using DashboardDomain.Features.CQRS.Query;
using DashboardDomain.Features.DTO.Query;
using DashboardEntity.Entities;
using Flaminco.CommitResult;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainEntities = DashboardEntity.Entities;

namespace DashboardInfrastructure.Features.AppSetting.CQRS.Query;

public class GetAppSettingByIdQueryHandler : IRequestHandler<GetAppSettingByIdQuery, ICommitResult<AppSettingResponse>>
{
    private readonly DashboardDbContext _dbContext;

    public GetAppSettingByIdQueryHandler(DashboardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ICommitResult<AppSettingResponse>> Handle(GetAppSettingByIdQuery request, CancellationToken cancellationToken)
    {
        return ResultType.Ok.GetValueCommitResult(await _dbContext.Set<DomainEntities.AppSetting>().Where(a => a.Id.Equals(request.Id)).ProjectToType<AppSettingResponse>().SingleOrDefaultAsync(cancellationToken));
    }
}