using CurriculumDomain.Features.Subjects.DTO;

namespace CurriculumDomain.Features.Subjects.CQRS.Query;

public record GetSubjectByFiltersQuery(int Grade, int TermId) : IRequest<CommitResults<SubjectProfileResponse>>;

