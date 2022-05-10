using SharedModule.DTO;

namespace CurriculumDomain.Features.Subjects.GetSubjectBrief.CQRS.Query;
public record GetSubjectBriefQuery(string SubjectId) : IRequest<CommitResult<SubjectBriefResponse>>;