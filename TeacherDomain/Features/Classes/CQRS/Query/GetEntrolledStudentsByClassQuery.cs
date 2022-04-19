using TeacherDomain.Features.Classes.DTO.Query;

namespace TeacherDomain.Features.Classes.CQRS.Query;
public record GetEntrolledStudentsByClassQuery(int ClassId) : IRequest<CommitResults<EnrolledStudentResponse>>;


