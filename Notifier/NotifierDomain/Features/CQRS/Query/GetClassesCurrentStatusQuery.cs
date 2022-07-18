using Flaminco.CommitResult;
using SharedModule.DTO;

namespace NotifierDomain.Features.CQRS.Query;

public record GetClassesCurrentStatusQuery(IEnumerable<int> ClassIds) : IRequest<ICommitResults<ClassStatusResponse>>;


