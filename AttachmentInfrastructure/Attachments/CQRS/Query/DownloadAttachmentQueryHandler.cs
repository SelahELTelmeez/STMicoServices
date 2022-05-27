using AttachmentDomain.Features.Attachments.CQRS.Query;
using AttachmentEntities.Entities.Attachments;
using AttachmentEntity;
using Flaminco.CommitResult;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace AttachmentInfrastructure.Features.Attachments.CQRS.Query
{
    public class DownloadAttachmentQueryHandler : IRequestHandler<DownloadAttachmentQuery, ICommitResult<string>>
    {
        private readonly AttachmentDbContext _dbContext;
        private readonly string _attachmentPath;
        public DownloadAttachmentQueryHandler(AttachmentDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _attachmentPath = Path.Combine(webHostEnvironment.WebRootPath, "Attachments");
        }
        public async Task<ICommitResult<string>> Handle(DownloadAttachmentQuery request, CancellationToken cancellationToken)
        {
            Attachment? attachment = await _dbContext.Set<Attachment>().SingleOrDefaultAsync(a => a.Id.Equals(request.Id), cancellationToken);
            if (attachment == null)
            {
                return Flaminco.CommitResult.ResultType.NotFound.GetValueCommitResult(default(string), "X0000", "X0000");
            }
            return Flaminco.CommitResult.ResultType.Ok.GetValueCommitResult(Path.Combine(_attachmentPath, attachment.FullName));
        }
    }
}
