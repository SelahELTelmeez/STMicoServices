using SharedModule.DTO;

namespace CurriculumDomain.Features.Subjects.GetSubjects.CQRS.Query;

public record GetSubjectsBriefByTermQuery(int Grade, int TermId) : IRequest<CommitResults<SubjectBriefResponse>>;


