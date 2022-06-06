using TeacherDomain.Features.Classes.DTO.Query;

namespace TeacherDomain.Features.Classes.CQRS.Query;

public record GetEnrolleeDetailsQuery(Guid EnrolleeId) : IRequest<ICommitResult<EnrolleeDetailsResponse>>;


