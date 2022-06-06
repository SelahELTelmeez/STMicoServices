using Flaminco.CommitResult;
using MediatR;

namespace DashboardDomain.Features.CQRS.Command;

public record DeleteSectionGroupCommand(int Id) : IRequest<ICommitResult>;

