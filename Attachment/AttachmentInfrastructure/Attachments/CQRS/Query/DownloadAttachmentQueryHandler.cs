using AttachmentDomain.Features.Attachments.CQRS.Query;
using AttachmentEntities.Entities.Attachments;
using AttachmentEntity;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace AttachmentInfrastructure.Attachments.CQRS.Query;

public class DownloadAttachmentQueryHandler : IRequestHandler<DownloadAttachmentQuery, ICommitResult<string>>
{
    private readonly AttachmentDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly string _currentBaseUrl;
    private readonly IDistributedCache _cache;

    public DownloadAttachmentQueryHandler(AttachmentDbContext dbContext, IWebHostEnvironment configuration,
                                          IHttpContextAccessor httpContextAccessor, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _currentBaseUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
        _cache = cache;
    }
    public async Task<ICommitResult<string>> Handle(DownloadAttachmentQuery request, CancellationToken cancellationToken)
    {
        Attachment? cachedAttachment = await _cache.GetFromCacheAsync<Guid, Attachment>(request.Id, "Attachment", cancellationToken);

        if (cachedAttachment == null)
        {
            cachedAttachment = await _dbContext.Set<Attachment>().FirstOrDefaultAsync(a => a.Id.Equals(request.Id), cancellationToken);
            if (cachedAttachment == null)
            {
                return ResultType.NotFound.GetValueCommitResult(string.Empty, "XATC0002", _resourceJsonManager["XATC0002"]);
            }

            await _cache.SaveToCacheAsync(cachedAttachment.Id, cachedAttachment, "Attachment", cancellationToken);

        }
        return ResultType.Ok.GetValueCommitResult(Path.Combine(_currentBaseUrl, "Attachment", "Attachments", cachedAttachment.FullName).Replace("\\", "/"));
    }
}
