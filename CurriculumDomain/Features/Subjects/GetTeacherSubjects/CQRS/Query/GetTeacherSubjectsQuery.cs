using CurriculumDomain.Features.Subjects.GetTeacherSubjects.DTO;

namespace CurriculumDomain.Features.Subjects.GetTeacherSubjects.CQRS.Query;

public record GetTeacherSubjectsQuery(IEnumerable<string> subjectIds) : IRequest<CommitResults<TeacherSubjectReponse>>;


