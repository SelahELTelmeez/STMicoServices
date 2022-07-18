using Flaminco.CommitResult;
using SharedModule.DTO;

namespace IdentityDomain.Features.GradesDropDown.CQRS.Query;

public record GetGradesQuery : IRequest<ICommitResults<GradeResponse>>;


