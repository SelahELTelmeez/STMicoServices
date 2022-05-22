using SharedModule.DTO;

namespace CurriculumDomain.Features.Reports.CQRS.Query;

public record GetSubjectsBriefProgressQuery(int Term, Guid? StudentId) : IRequest<CommitResults<SubjectBriefProgressResponse>>;
