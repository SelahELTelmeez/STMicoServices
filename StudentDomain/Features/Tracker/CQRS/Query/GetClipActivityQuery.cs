using SharedModule.DTO;

namespace StudentDomain.Features.Tracker.CQRS.Query;
public record GetClipActivityQuery(List<int> ClipIds) : IRequest<CommitResults<ClipActivityResponse>>;