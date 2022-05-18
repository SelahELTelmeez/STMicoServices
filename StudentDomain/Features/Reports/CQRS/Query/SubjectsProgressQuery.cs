using SharedModule.DTO;

namespace StudentDomain.Features.Reports.CQRS.Query;

public record SubjectsProgressQuery(int Term) : IRequest<CommitResults<SubjectBriefProgressResponse>>;

