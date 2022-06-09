using AttachmentDomain.Features.Attachments.CQRS.Command;
using AttachmentEntities.Entities.Attachments;
using AttachmentEntity;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace AttachmentInfrastructure.Features.Attachments.CQRS.Command
{
    public class UploadAttachmentCommandHandler : IRequestHandler<UploadAttachmentCommand, ICommitResult<Guid>>
    {
        private readonly string _attachmentPath;
        private readonly AttachmentDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;

        public UploadAttachmentCommandHandler(AttachmentDbContext dbContext,
                                              IWebHostEnvironment configuration,
                                              IHttpContextAccessor httpContextAccessor)
        {
            _attachmentPath = Path.Combine(configuration.WebRootPath, "Attachments");
            _dbContext = dbContext;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        }
        public async Task<ICommitResult<Guid>> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
        {
            if (request.FormFile == null)
            {
                return ResultType.Invalid.GetValueCommitResult(Guid.Empty, "X0001", _resourceJsonManager["X0001"]);
            }
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

            return ResultType.Ok.GetValueCommitResult(attachment.Id);
        }
    }
}
