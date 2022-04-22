using Microsoft.AspNetCore.Mvc;
using TeacherDomain.Features.Attachments.CQRS.Query;
using TeacherEntities.Entities.Attachments;

namespace TeacherInfrastructure.Features.Attachments.CQRS.Query
{
    public class DownloadAttachmentQueryHandler : IRequestHandler<DownloadAttachmentQuery, CommitResult<FileStreamResult>>
    {
        private readonly TeacherDbContext _dbContext;
        private readonly string _attachmentPath;
        public DownloadAttachmentQueryHandler(TeacherDbContext dbContext, IWebHostEnvironment webHostEnvironment)
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
