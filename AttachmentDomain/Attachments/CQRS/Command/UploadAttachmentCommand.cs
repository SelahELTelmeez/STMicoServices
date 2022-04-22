using MediatR;
using Microsoft.AspNetCore.Http;
using ResultHandler;

namespace AttachmentDomain.Features.Attachments.CQRS.Command;

public record UploadAttachmentCommand(IFormFile FormFile) : IRequest<CommitResult<Guid>>;
