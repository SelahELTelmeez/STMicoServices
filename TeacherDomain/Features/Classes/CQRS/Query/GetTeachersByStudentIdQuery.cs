using SharedModule.DTO;

namespace TeacherDomain.Features.Classes.CQRS.Query;

public record GetTeachersByStudentIdQuery(Guid StudentId) : IRequest<CommitResults<LimitedProfileResponse>>;