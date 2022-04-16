using TransactionDomain.Features.Classes.DTO.Query;

namespace TransactionDomain.Features.Classes.CQRS.Query;
public record SearchClassBySubjectQuery(string SubjectId) : IRequest<CommitResults<ClassResponse>>;