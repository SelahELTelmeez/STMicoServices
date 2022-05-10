using IdentityDomain.Features.GradesDropDown.DTO;
using ResultHandler;

namespace IdentityDomain.Features.GradesDropDown.CQRS.Query;

public record GetGradeByIdQuery(int GradeId) : IRequest<CommitResult<GradeResponse>>;

