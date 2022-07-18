using SharedModule.DTO;

namespace CurriculumDomain.Features.Reports.CQRS.Query;

public record GetSubjectsBriefProgressQuery(int Term, string? StudentId) : IRequest<CommitResults<SubjectBriefProgressResponse>>;
