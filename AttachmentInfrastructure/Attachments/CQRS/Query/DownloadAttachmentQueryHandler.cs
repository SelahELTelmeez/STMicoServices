using AttachmentDomain.Features.Attachments.CQRS.Query;
using AttachmentEntities.Entities.Attachments;
using AttachmentEntity;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AttachmentInfrastructure.Features.Attachments.CQRS.Query
{
    public class DownloadAttachmentQueryHandler : IRequestHandler<DownloadAttachmentQuery, ICommitResult<string>>
    {
        private readonly AttachmentDbContext _dbContext;
        private readonly string _attachmentPath;
        private readonly JsonLocalizerManager _resourceJsonManager;
        public DownloadAttachmentQueryHandler(AttachmentDbContext dbContext, IWebHostEnvironment configuration,
                                              IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _attachmentPath = Path.Combine(configuration.WebRootPath, "Attachments");
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        }
        public async Task<ICommitResult<string>> Handle(DownloadAttachmentQuery request, CancellationToken cancellationToken)
        {
            Attachment? attachment = await _dbContext.Set<Attachment>().SingleOrDefaultAsync(a => a.Id.Equals(request.Id), cancellationToken);
            if (attachment == null)
            {
                return ResultType.NotFound.GetValueCommitResult(string.Empty, "X0002", _resourceJsonManager["X0002"]);
            }
            return ResultType.Ok.GetValueCommitResult(Path.Combine(_attachmentPath, attachment.FullName));
        }
    }
}
