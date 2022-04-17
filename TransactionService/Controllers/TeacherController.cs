using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionDomain.Features.Classes.CQRS.Command;
using TransactionDomain.Features.TeacherClass.DTO.Command;

namespace TransactionService.Controllers;

[ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TeacherController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeacherController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> CreateClass([FromBody] CreateClassRequest createClassRequest, CancellationToken token)
        => Ok(await _mediator.Send(new CreateClassCommand(createClassRequest), token));

}
