namespace CurriculumDomain.Features.Subjects.VerifySubjectStudentGradeMatching.CQRS.Query;

public record VerifySubjectStudentGradeMatchingQuery(string SubjectId, int GradeId) : IRequest<CommitResult<bool>>;


