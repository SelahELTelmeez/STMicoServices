using TransactionDomain.Features.TeacherClass.DTO.Query;

namespace TransactionDomain.Features.TeacherClass.CQRS.Query;

public record GetTeacherClassesBySubjectQuery(string SubjectId) : IRequest<CommitResults<TeacherClassResponse>>;


