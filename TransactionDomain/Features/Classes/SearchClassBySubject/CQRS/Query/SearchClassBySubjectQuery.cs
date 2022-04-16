using TransactionDomain.Features.Classes.SearchClassBySubject.DTO.Query;

namespace TransactionDomain.Features.Classes.SearchClassBySubject.CQRS.Query;
public record SearchClassBySubjectQuery(string SubjectId) : IRequest<CommitResults<SearchClassBySubjectResponse>>;