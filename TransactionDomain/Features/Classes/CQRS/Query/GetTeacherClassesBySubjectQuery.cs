using TransactionDomain.Features.TeacherClass.DTO.Query;

namespace TransactionDomain.Features.Classes.CQRS.Query;

public record GetTeacherClassesBySubjectQuery(string SubjectId) : IRequest<CommitResults<TeacherClassResponse>>;


