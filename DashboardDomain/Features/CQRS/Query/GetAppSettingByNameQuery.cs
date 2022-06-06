using DashboardDomain.Features.DTO.Query;
using Flaminco.CommitResult;
using MediatR;

namespace DashboardDomain.Features.CQRS.Query;

public record GetAppSettingByNameQuery(string Name) : IRequest<ICommitResult<AppSettingResponse>>;

