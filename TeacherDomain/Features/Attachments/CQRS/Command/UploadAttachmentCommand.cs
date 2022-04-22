using Microsoft.AspNetCore.Http;

namespace TeacherDomain.Features.Attachments.CQRS.Command
{
    public record UploadAttachmentCommand(IFormFile FormFile) : IRequest<CommitResult<Guid>>;
}
