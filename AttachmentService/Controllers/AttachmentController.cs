﻿using AttachmentDomain.Features.Attachments.CQRS.Command;
using AttachmentDomain.Features.Attachments.CQRS.Query;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResultHandler;

namespace AttachmentService.Controllers;

[ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AttachmentController : Controller
{
    private readonly IMediator _mediator;
    public AttachmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new UploadAttachmentCommand(file), cancellationToken));

    [HttpGet("[action]")]
    public async Task<IActionResult> Download(Guid Id, CancellationToken cancellationToken)
    {
        CommitResult<FileStreamResult> commitResult = await _mediator.Send(new DownloadAttachmentQuery(Id), cancellationToken);
        if (commitResult.IsSuccess)
        {
            return commitResult.Value;
        }
        else
        {
            return Ok(commitResult);
        }
    }

}