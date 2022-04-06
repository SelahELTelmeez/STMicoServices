using MediatR;
using ResultHandler;
using TransactionDomain.Features.Tracker.DTO.Command;

namespace TransactionDomain.Features.Tracker.CQRS.Command;

public record UpdateActivityCommand(UpdateActivityRequest ActivityRequest) : IRequest<CommitResult>;
