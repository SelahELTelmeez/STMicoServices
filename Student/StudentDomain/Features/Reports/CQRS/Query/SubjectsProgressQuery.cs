using SharedModule.DTO;

namespace StudentDomain.Features.Reports.CQRS.Query;

public record SubjectsProgressQuery(int Term, string? StudentId) : IRequest<ICommitResults<SubjectBriefProgressResponse>>;

