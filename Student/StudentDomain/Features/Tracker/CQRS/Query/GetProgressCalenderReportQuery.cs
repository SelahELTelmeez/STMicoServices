using StudentDomain.Features.Tracker.DTO.Query;

namespace StudentDomain.Features.Tracker.CQRS.Query;

public record GetProgressCalenderReportQuery(string? StudentId) : IRequest<ICommitResult<ProgressCalenderResponse>>;




