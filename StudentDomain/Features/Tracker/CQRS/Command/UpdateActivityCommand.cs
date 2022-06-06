using StudentDomain.Features.Tracker.DTO.Command;

namespace StudentDomain.Features.Tracker.CQRS.Command;

public record UpdateActivityCommand(UpdateActivityRequest ActivityRequest) : IRequest<ICommitResult>;
