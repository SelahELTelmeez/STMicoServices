using SharedModule.DTO;

namespace StudentDomain.Features.Reports.CQRS.Query;

public record RecentActivityQuery(int Term, string? StudentId) : IRequest<ICommitResults<RecentActivityResponse>>;
