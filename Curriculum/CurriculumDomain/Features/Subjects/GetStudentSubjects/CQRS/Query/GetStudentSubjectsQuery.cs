using CurriculumDomain.Features.Subjects.GetStudentSubjects.DTO.Query;
namespace CurriculumDomain.Features.Subjects.GetStudentSubjects.CQRS.Query;
public record GetStudentSubjectsQuery(string? StudentId) : IRequest<CommitResults<IdnentitySubjectResponse>>;