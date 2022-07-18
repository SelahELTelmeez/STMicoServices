using Flaminco.CommitResult;
using MediatR;

namespace DashboardDomain.Features.CQRS.Command;

public record UpdateSectionGroupCommand(int Id, string Name) : IRequest<ICommitResult>;

