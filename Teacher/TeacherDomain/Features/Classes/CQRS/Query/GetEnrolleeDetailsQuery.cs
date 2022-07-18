using TeacherDomain.Features.Classes.DTO.Query;

namespace TeacherDomain.Features.Classes.CQRS.Query;

public record GetEnrolleeDetailsQuery(string EnrolleeId) : IRequest<ICommitResult<EnrolleeDetailsResponse>>;


