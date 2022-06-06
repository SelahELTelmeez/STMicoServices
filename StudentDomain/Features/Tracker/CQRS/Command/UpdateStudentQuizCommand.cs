using StudentDomain.Features.Tracker.DTO.Command;

namespace StudentDomain.Features.Tracker.CQRS.Command;
public record UpdateStudentQuizCommand(UpdateStudentQuizRequest UpdateStudentQuizRequest) : IRequest<ICommitResult>;


