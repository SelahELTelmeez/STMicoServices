using IdentityDomain.Features.GradesDropDown.DTO;
using ResultHandler;

namespace IdentityDomain.Features.GradesDropDown.CQRS.Query;

public record GetGradeByIdsQuery(IEnumerable<int> GradeIds) : IRequest<CommitResults<GradeResponse>>;

