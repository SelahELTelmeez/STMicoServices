using MediatR;
using ResultHandler;

namespace AttachmentDomain.Features.Attachments.CQRS.Query;

public record DownloadAttachmentQuery(Guid Id) : IRequest<CommitResult<string>>;
