using Microsoft.AspNetCore.Mvc;

namespace TeacherDomain.Features.Attachments.CQRS.Query;

public record DownloadAttachmentQuery(Guid Id) : IRequest<CommitResult<FileStreamResult>>;
