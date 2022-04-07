using CurriculumDomain.Features.Subjects.GetStudentSubjects.DTO.Query;
using ResultHandler;
namespace CurriculumDomain.Features.Subjects.GetStudentSubjects.CQRS.Query;
public record GetStudentSubjectsQuery() : IRequest<CommitResults<IdnentitySubjectResponse>>;