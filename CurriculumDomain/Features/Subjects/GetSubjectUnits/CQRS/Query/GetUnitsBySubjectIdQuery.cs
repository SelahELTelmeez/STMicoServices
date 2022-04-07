using CurriculumDomain.Features.Subjects.GetSubjectUnits.DTO.Query;
using ResultHandler;

namespace CurriculumDomain.Features.Subjects.GetSubjectUnits.CQRS.Query;
public record GetUnitsBySubjectIdQuery(string SubjectId) : IRequest<CommitResults<UnitResponse>>;