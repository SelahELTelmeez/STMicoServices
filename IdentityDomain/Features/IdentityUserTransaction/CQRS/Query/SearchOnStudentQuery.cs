using IdentityDomain.Features.IdentityUserTransaction.DTO;
using ResultHandler;

namespace IdentityDomain.Features.IdentityUserTransaction.CQRS.Query;

public record SearchOnStudentQuery(SearchOnStudentRequest SearchOnStudentRequest) : IRequest<CommitResult<SearchOnStudentResponse>>;