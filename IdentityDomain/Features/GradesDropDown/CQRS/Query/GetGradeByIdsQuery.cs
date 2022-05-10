using ResultHandler;
using SharedModule.DTO;

namespace IdentityDomain.Features.GradesDropDown.CQRS.Query;

public record GetGradeByIdsQuery(IEnumerable<int> GradeIds) : IRequest<CommitResults<GradeResponse>>;

