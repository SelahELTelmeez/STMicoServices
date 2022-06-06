using Flaminco.CommitResult;
using MediatR;

namespace DashboardDomain.Features.CQRS.Command;

public record DeleteAppSettingCommand(int Id) : IRequest<ICommitResult>;

