using Flaminco.CommitResult;
using MediatR;

namespace DashboardDomain.Features.CQRS.Command;

public record InsertSectionGroupCommand(string Name) : IRequest<ICommitResult>;


