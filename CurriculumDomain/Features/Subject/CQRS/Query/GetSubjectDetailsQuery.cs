using CurriculumDomain.Features.Subject.DTO.Query;
using ResultHandler;

namespace CurriculumDomain.Features.Subject.CQRS.Query;
public record GetSubjectDetailsQuery(string SubjectId) : IRequest<CommitResults<SubjectDetailsResponse>>;