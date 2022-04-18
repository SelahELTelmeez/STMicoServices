using TeacherDomain.Features.Classes.DTO.Query;

namespace TeacherDomain.Features.Classes.CQRS.Query;

public record GetActivitiesByClassQuery(int ClassId) : IRequest<CommitResults<ClassActivityResponse>>;
