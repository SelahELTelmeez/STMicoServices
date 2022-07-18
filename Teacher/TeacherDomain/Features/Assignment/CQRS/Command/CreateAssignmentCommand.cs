using MediatR;
using ResultHandler;
using TeacherDomain.Features.Assignment.DTO.Command;

namespace TeacherDomain.Features.Assignment.CQRS.Command;
public record CreateAssignmentCommand(CreateAssignmentRequest CreateAssignmentRequest) : IRequest<ICommitResult>;