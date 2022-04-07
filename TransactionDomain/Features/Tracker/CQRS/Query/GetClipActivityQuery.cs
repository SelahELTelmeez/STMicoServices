using TransactionDomain.Features.Tracker.DTO.Query;

namespace TransactionDomain.Features.Tracker.CQRS.Query;
public record GetClipActivityQuery(List<int> ClipIds) : IRequest<CommitResults<ClipActivityResponse>>;