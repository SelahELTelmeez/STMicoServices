using TransactionDomain.Features.Parent.DTO.Query;

namespace TransactionDomain.Features.Parent.CQRS.Query;

public record GetParentHomeDataQuery(Guid StudentId):IRequest<CommitResults<ClassesEntrolledByStudentResponse>>;
