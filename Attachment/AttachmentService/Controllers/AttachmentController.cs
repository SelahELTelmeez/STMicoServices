using AttachmentDomain.Features.Attachments.CQRS.Command;
using AttachmentDomain.Features.Attachments.CQRS.Query;
using Flaminco.CommitResult;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttachmentService.Controllers;

[ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AttachmentController : Controller
{
    private readonly IMediator _mediator;
    public AttachmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("[action]"), Produces(typeof(ICommitResult<Guid>))]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new UploadAttachmentCommand(file), cancellationToken));

    [HttpGet("[action]"), Produces(typeof(ICommitResult<string>))]
    public async Task<IActionResult> Download(Guid Id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new DownloadAttachmentQuery(Id), cancellationToken));


}
