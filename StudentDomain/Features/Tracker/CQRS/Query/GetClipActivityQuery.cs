using StudentDomain.Features.Tracker.DTO.Query;

namespace StudentDomain.Features.Tracker.CQRS.Query;
public record GetClipActivityQuery(List<int> ClipIds) : IRequest<CommitResults<ClipActivityResponse>>;