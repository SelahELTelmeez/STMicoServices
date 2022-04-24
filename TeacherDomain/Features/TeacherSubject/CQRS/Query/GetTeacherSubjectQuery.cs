using TeacherDomain.Features.TeacherSubject.DTO.Query;

namespace TeacherDomain.Features.TeacherSubject.CQRS.Query;
public record GetTeacherSubjectQuery() : IRequest<CommitResults<TeacherSubjectResponse>>;
