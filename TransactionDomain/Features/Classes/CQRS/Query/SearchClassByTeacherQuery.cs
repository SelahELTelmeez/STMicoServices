using TransactionDomain.Features.Classes.DTO.Query;

namespace TransactionDomain.Features.Classes.CQRS.Query;
public record SearchClassByTeacherQuery(string NameOrMobile) : IRequest<CommitResults<ClassResponse>>;