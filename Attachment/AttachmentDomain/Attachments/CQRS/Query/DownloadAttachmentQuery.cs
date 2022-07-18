using Flaminco.CommitResult;
using MediatR;

namespace AttachmentDomain.Features.Attachments.CQRS.Query;

public record DownloadAttachmentQuery(Guid Id) : IRequest<ICommitResult<string>>;
