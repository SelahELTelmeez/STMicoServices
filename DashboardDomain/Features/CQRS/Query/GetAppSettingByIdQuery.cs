using DashboardDomain.Features.DTO.Query;
using Flaminco.CommitResult;
using MediatR;

namespace DashboardDomain.Features.CQRS.Query;

public record GetAppSettingByIdQuery(int Id) : IRequest<ICommitResult<AppSettingResponse>>;

