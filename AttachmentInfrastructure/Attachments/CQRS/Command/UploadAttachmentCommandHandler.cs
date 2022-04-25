using AttachmentDomain.Features.Attachments.CQRS.Command;
using AttachmentEntities.Entities.Attachments;
using AttachmentEntity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace AttachmentInfrastructure.Features.Attachments.CQRS.Command
{
    public class UploadAttachmentCommandHandler : IRequestHandler<UploadAttachmentCommand, CommitResult<Guid>>
    {
        private readonly string _attachmentPath;
        private readonly AttachmentDbContext _dbContext;
        public UploadAttachmentCommandHandler(IWebHostEnvironment webHostEnvironment, AttachmentDbContext dbContext)
        {
            _attachmentPath = Path.Combine(webHostEnvironment.WebRootPath, "Attachments");
            _dbContext = dbContext;
        }
        public async Task<CommitResult<Guid>> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
        {
            Stream fileOpenStream = request.FormFile.OpenReadStream();

            string checksum = fileOpenStream.ComputeMD5Hash();

            Attachment? attachment = await _dbContext.Set<Attachment>().SingleOrDefaultAsync(a => a.Checksum == checksum, cancellationToken);

            if (attachment == null)
            {
                string fileExtension = Path.GetExtension(request.FormFile.FileName);
                string fileRandomName = Path.GetRandomFileName();
                string fileName = fileRandomName + fileExtension;
                string fileNameWithPath = Path.Combine(_attachmentPath, fileName);

                new FileExtensionContentTypeProvider().TryGetContentType(fileName, out string? contentType);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    await request.FormFile.CopyToAsync(stream, cancellationToken);
                }

                attachment = new Attachment
                {
                    Id = Guid.NewGuid(),
                    Checksum = checksum,
                    Extension = fileExtension,
                    Name = fileRandomName,
                    MineType = contentType ?? "application/octet-stream"
                };

                _dbContext.Set<Attachment>().Add(attachment);

                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return new CommitResult<Guid>
            {
                ResultType = ResultType.Ok,
                Value = attachment.Id
            };

        }
    }
}
