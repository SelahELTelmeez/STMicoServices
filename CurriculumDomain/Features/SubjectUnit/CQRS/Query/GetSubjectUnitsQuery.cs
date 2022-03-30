using CurriculumDomain.Features.SubjectUnit.DTO.Query;
using ResultHandler;

namespace CurriculumDomain.Features.SubjectUnit.CQRS.Query;
public record GetSubjectUnitsQuery(string CurriculumId) : IRequest<CommitResult<List<SubjectUnitResponse>>>;