using SharedModule.DTO;

namespace TeacherDomain.Features.Classes.CQRS.Query;

public record GetTeachersByStudentIdQuery(string StudentId) : IRequest<ICommitResults<LimitedTeacherProfileResponse>>;