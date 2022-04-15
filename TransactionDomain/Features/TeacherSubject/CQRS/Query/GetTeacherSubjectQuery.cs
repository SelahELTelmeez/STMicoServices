using TransactionDomain.Features.TeacherSubject.DTO.Query;

namespace TransactionDomain.Features.TeacherSubject.CQRS.Query;

public record GetTeacherSubjectQuery() : IRequest<CommitResults<TeacherSubjectReponse>>;
