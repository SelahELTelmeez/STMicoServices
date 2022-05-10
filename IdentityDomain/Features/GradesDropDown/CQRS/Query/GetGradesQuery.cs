using ResultHandler;
using SharedModule.DTO;

namespace IdentityDomain.Features.GradesDropDown.CQRS.Query;

public record GetGradesQuery : IRequest<CommitResults<GradeResponse>>;


