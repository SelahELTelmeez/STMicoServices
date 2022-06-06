using StudentDomain.Features.Activities.DTO.Command;

namespace StudentDomain.Features.Activities.CQRS.Command;

public record InsertActivityCommand(InsertActivityRequest ActivityRequest) : IRequest<ICommitResult<int>>;
