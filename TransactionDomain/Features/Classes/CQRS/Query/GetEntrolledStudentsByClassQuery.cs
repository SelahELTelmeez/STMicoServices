using TransactionDomain.Features.Classes.DTO.Query;

namespace TransactionDomain.Features.Classes.CQRS.Query;

public record GetEntrolledStudentsByClassQuery(int ClassId) : IRequest<CommitResults<EnrolledStudentResponse>>;


