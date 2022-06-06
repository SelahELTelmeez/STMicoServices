using MediatR;
using ResultHandler;
using TeacherDomain.Features.Assignment.DTO.Query;

namespace TeacherDomain.Features.Assignment.CQRS.Query;
public record class GetAssignmentsQuery() : IRequest<ICommitResults<AssignmentResponse>>;


