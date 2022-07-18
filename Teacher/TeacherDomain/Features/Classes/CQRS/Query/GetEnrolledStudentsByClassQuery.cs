using TeacherDomain.Features.Classes.DTO.Query;

namespace TeacherDomain.Features.Classes.CQRS.Query;
public record GetEnrolledStudentsByClassQuery(int ClassId) : IRequest<ICommitResults<EnrolledStudentResponse>>;


