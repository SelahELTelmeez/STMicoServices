using TransactionDomain.Features.TeacherClass.DTO.Query;

namespace TransactionDomain.Features.TeacherClass.CQRS.Query;

public record GetStudentClassesQuery() : IRequest<CommitResults<StudentClassResponse>>;


