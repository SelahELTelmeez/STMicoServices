using CurriculumDomain.Features.Subjects.GetSubjectDetails.DTO.Query;
using ResultHandler;

namespace CurriculumDomain.Features.Subjects.GetSubjectDetails.CQRS.Query;
public record GetSubjectDetailsQuery(string SubjectId) : IRequest<CommitResult<SubjectDetailsResponse>>;