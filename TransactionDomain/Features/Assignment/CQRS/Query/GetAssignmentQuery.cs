using TransactionDomain.Features.Assignment.DTO.Query;

namespace TransactionDomain.Features.Assignment.CQRS.Query;

public record class GetAssignmentQuery() : IRequest<CommitResults<AssignmentResponse>>;


