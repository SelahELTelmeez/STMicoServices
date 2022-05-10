using AttachmentDomain.Features.Attachments.CQRS.Query;
using AttachmentEntities.Entities.Attachments;
using AttachmentEntity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace AttachmentInfrastructure.Features.Attachments.CQRS.Query
{
    public class DownloadAttachmentQueryHandler : IRequestHandler<DownloadAttachmentQuery, CommitResult<string>>
    {
        private readonly AttachmentDbContext _dbContext;
        private readonly string _attachmentPath;
        public DownloadAttachmentQueryHandler(AttachmentDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _attachmentPath = Path.Combine(webHostEnvironment.WebRootPath, "Attachments");
        }
        public async Task<CommitResult<string>> Handle(DownloadAttachmentQuery request, CancellationToken cancellationToken)
        {
            Attachment? attachment = await _dbContext.Set<Attachment>().SingleOrDefaultAsync(a => a.Id.Equals(request.Id), cancellationToken);
            if (attachment == null)
            {
                return new CommitResult<string>()
                {
                    ResultType = ResultType.NotFound,
                    ErrorCode = "X0000",
                    ErrorMessage = "X00000"
                };
            }

            return new CommitResult<string>()
            {
                ResultType = ResultType.Ok,
                Value = Path.Combine(_attachmentPath, attachment.FullName)
            };
        }
    }
}
