using Flaminco.CommitResult;
using MediatR;

namespace DashboardDomain.Features.CQRS.Command;

public record UpdateSectionCommand : IRequest<CommitResult>;


