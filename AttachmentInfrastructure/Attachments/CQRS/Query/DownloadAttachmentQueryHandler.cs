using AttachmentDomain.Features.Attachments.CQRS.Query;
using AttachmentEntities.Entities.Attachments;
using AttachmentEntity;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace AttachmentInfrastructure.Features.Attachments.CQRS.Query
{
    public class DownloadAttachmentQueryHandler : IRequestHandler<DownloadAttachmentQuery, CommitResult<FileStreamResult>>
    {
        private readonly AttachmentDbContext _dbContext;
        private readonly string _attachmentPath;
        public DownloadAttachmentQueryHandler(AttachmentDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _attachmentPath = Path.Combine(webHostEnvironment.WebRootPath, "Attachments");
        }
        public async Task<CommitResult<FileStreamResult>> Handle(DownloadAttachmentQuery request, CancellationToken cancellationToken)
        {
            Attachment? attachment = await _dbContext.Set<Attachment>().SingleOrDefaultAsync(a => a.Id.Equals(request.Id), cancellationToken);
            if (attachment == null)
            {
                return new CommitResult<FileStreamResult>()
                {
                    ResultType = ResultType.NotFound,
                    ErrorCode = "X0000",
                    ErrorMessage = "X00000"
                };
            }

            return new CommitResult<FileStreamResult>()
            {
                ResultType = ResultType.Ok,
                Value = new FileStreamResult(new FileStream(Path.Combine(_attachmentPath, attachment.FullName), FileMode.Open, FileAccess.Read), attachment.MineType)
                {
                    FileDownloadName = attachment.FullName,
                }
            };
        }
    }
}
