using CurriculumDomain.Features.Subjects.GetSubjects.DTO.Query;

namespace CurriculumDomain.Features.Subjects.GetSubjects.CQRS.Query;
public record GetSubjectsQuery(IEnumerable<string> SubjectIds) : IRequest<CommitResults<SubjectResponse>>;