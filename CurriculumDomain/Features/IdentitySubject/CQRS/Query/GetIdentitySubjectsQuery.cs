using CurriculumDomain.Features.IdnentitySubject.DTO.Query;
using ResultHandler;
namespace CurriculumDomain.Features.IdnentitySubject.CQRS.Query;
public record GetIdentitySubjectsQuery() : IRequest<CommitResult<List<IdnentitySubjectResponse>>>;