using Flaminco.CommitResult;
using MediatR;

namespace DashboardDomain.Features.CQRS.Command;

public record DeleteSectionCommand(int Id) : IRequest<ICommitResult>;

