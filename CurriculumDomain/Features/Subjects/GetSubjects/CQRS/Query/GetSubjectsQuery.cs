using SharedModule.DTO;

namespace CurriculumDomain.Features.Subjects.GetSubjects.CQRS.Query;
public record GetSubjectsQuery(IEnumerable<string> SubjectIds) : IRequest<CommitResults<SubjectResponse>>;