using Flaminco.CommitResult;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AttachmentDomain.Features.Attachments.CQRS.Command;

public record UploadAttachmentCommand(IFormFile FormFile) : IRequest<ICommitResult<Guid>>;
