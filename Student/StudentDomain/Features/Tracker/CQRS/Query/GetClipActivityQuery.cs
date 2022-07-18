using SharedModule.DTO;

namespace StudentDomain.Features.Tracker.CQRS.Query;
public record GetClipActivityQuery(IEnumerable<int> ClipIds) : IRequest<ICommitResults<ClipActivityResponse>>;