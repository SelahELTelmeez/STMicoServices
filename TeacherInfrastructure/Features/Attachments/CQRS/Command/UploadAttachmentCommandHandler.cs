using Microsoft.AspNetCore.StaticFiles;
using TeacherDomain.Features.Attachments.CQRS.Command;
using TeacherEntities.Entities.Attachments;

namespace TeacherInfrastructure.Features.Attachments.CQRS.Command
{
    public class UploadAttachmentCommandHandler : IRequestHandler<UploadAttachmentCommand, CommitResult<Guid>>
    {
        private readonly string _attachmentPath;
        private readonly TeacherDbContext _dbContext;
        public UploadAttachmentCommandHandler(IWebHostEnvironment webHostEnvironment, TeacherDbContext dbContext)
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
