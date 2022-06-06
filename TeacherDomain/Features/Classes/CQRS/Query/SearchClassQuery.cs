using TeacherDomain.Features.Classes.DTO.Query;

namespace TeacherDomain.Features.Classes.CQRS.Query;
public record SearchClassQuery(int ClassId) : IRequest<ICommitResult<ClassResponse>>;