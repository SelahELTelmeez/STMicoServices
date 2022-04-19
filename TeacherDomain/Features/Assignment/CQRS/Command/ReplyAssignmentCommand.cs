using MediatR;
using ResultHandler;
using TeacherDomain.Features.Assignment.DTO.Command;

namespace TeacherDomain.Features.Assignment.CQRS.Command;

public record ReplyAssignmentCommand(ReplyAssignmentRequest ReplyAssignmentRequest) : IRequest<CommitResult>;

