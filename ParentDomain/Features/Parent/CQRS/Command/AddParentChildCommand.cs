using MediatR;
using ParentDomain.Features.Parent.DTO;
using ResultHandler;

namespace ParentDomain.Features.Parent.CQRS.Command;

public record AddParentChildCommand(AddParentChildRequest AddParentChildRequest) : IRequest<CommitResult<AddParentChildResponse>>;