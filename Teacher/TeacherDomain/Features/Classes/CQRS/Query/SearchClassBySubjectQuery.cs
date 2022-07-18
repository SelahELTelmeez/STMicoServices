using TeacherDomain.Features.Classes.DTO.Query;

namespace TeacherDomain.Features.Classes.CQRS.Query;
public record SearchClassBySubjectQuery(string SubjectId) : IRequest<ICommitResults<ClassResponse>>;