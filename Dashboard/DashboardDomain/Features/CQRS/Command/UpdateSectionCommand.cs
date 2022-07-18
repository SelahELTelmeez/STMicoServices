using DashboardDomain.Features.DTO.Command;
using Flaminco.CommitResult;
using MediatR;

namespace DashboardDomain.Features.CQRS.Command;

public record UpdateSectionCommand(UpdateSectionRequest UpdateSectionRequest) : IRequest<CommitResult>;


