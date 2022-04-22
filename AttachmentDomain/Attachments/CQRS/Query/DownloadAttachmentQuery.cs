using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResultHandler;

namespace AttachmentDomain.Features.Attachments.CQRS.Query;

public record DownloadAttachmentQuery(Guid Id) : IRequest<CommitResult<FileStreamResult>>;
