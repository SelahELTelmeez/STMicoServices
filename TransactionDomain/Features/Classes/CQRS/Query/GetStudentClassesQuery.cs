using TransactionDomain.Features.TeacherClass.DTO.Query;

namespace TransactionDomain.Features.Classes.CQRS.Query;

public record GetStudentClassesQuery() : IRequest<CommitResults<StudentClassResponse>>;


