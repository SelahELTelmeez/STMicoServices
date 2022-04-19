using TeacherDomain.Features.Classes.DTO.Query;

namespace TeacherDomain.Features.Classes.CQRS.Query;
public record SearchClassByTeacherQuery(string NameOrMobile) : IRequest<CommitResults<ClassResponse>>;