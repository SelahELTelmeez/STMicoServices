using Flaminco.CommitResult;
using SharedModule.DTO;

namespace IdentityDomain.Features.GradesDropDown.CQRS.Query;

public record GetGradeByIdQuery(int GradeId) : IRequest<ICommitResult<GradeResponse>>;

