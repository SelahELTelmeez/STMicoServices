using SharedModule.DTO;

namespace CurriculumDomain.Features.Subjects.GetSubjectBrief.CQRS.Query;

public record GetSubjectsBriefQuery(IEnumerable<string> SubjectIds) : IRequest<CommitResults<SubjectBriefResponse>>;
