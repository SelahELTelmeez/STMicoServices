using Flaminco.CommitResult;
using SharedModule.DTO;

namespace IdentityDomain.Features.GradesDropDown.CQRS.Query;

public record GetGradeByIdsQuery(IEnumerable<int> GradeIds) : IRequest<ICommitResults<GradeResponse>>;

