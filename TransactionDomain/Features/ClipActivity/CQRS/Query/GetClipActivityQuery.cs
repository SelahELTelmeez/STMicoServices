using MediatR;
using ResultHandler;
using TransactionDomain.Features.ClipActivity.DTO.Query;

namespace TransactionDomain.Features.ClipActivity.CQRS.Query;
public record GetClipActivityQuery(List<int> ClipIds) : IRequest<CommitResult<IEnumerable<ClipActivityResponse>>>;