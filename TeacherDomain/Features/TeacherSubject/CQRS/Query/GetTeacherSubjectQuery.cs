using SharedModule.DTO;

namespace TeacherDomain.Features.TeacherSubject.CQRS.Query;
public record GetTeacherSubjectQuery() : IRequest<CommitResults<TeacherSubjectResponse>>;
