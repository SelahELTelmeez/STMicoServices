using SharedModule.DTO;

namespace CurriculumDomain.Features.Subjects.GetSubjectUnits.CQRS.Query;
public record GetUnitsBySubjectIdQuery(string SubjectId) : IRequest<CommitResults<UnitResponse>>;