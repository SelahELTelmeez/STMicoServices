using IdentityDomain.Features.GradesDropDown.DTO;
using ResultHandler;

namespace IdentityDomain.Features.GradesDropDown.CQRS.Query;

public record GetGradesQuery : IRequest<CommitResults<GradeResponse>>;


