using StudentDomain.Features.Tracker.DTO.Query;

namespace StudentDomain.Features.Tracker.CQRS.Query;

public record GetProgressCalenderReportQuery(Guid? StudentId) : IRequest<ICommitResult<ProgressCalenderResponse>>;




