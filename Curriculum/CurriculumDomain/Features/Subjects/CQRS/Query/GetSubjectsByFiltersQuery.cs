using CurriculumDomain.Features.Subjects.DTO;

namespace CurriculumDomain.Features.Subjects.CQRS.Query;

public record GetSubjectsByFiltersQuery(int Grade, int TermId) : IRequest<CommitResults<SubjectProfileResponse>>;

