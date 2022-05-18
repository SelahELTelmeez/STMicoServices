using SharedModule.DTO;

namespace CurriculumDomain.Features.Subjects.GetSubjects.CQRS.Query;

public record GetSubjectsDetailedQuery(IEnumerable<string> SubjectIds) : IRequest<CommitResults<SubjectDetailedResponse>>;

