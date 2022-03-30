using IdentityDomain.Features.GradesDropDown.DTO;
using ResultHandler;

namespace IdentityDomain.Features.GradesDropDown.CQRS.Query;

public record GetGradesDropDownMenuQuery : IRequest<CommitResult<List<GradeDropDownMenuItem>>>;


