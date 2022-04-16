using TransactionDomain.Features.Classes.SearchClassByTeacher.DTO.Query;

namespace TransactionDomain.Features.Classes.SearchClassByTeacher.CQRS.Query;
public record SearchClassByTeacherQuery() : IRequest<CommitResults<SearchClassByTeacherResponse>>;