using CurriculumDomain.Features.Subjects.GetTeacjerSubjects.DTO;

namespace CurriculumDomain.Features.Subjects.GetTeacjerSubjects.CQRS.Query;

public record GetTeacjerSubjectsQuery(IEnumerable<string> subjectIds) : IRequest<CommitResults<TeacherSubjectReponse>>;


