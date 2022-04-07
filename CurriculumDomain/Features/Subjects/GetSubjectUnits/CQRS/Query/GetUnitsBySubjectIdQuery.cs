using CurriculumDomain.Features.Subjects.GetSubjectUnits.DTO.Query;

namespace CurriculumDomain.Features.Subjects.GetSubjectUnits.CQRS.Query;
public record GetUnitsBySubjectIdQuery(string SubjectId) : IRequest<CommitResults<UnitResponse>>;