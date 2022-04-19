using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherDomain.Features.Classes.CQRS.Command;
using TeacherDomain.Features.Classes.DTO.Command;
using TeacherDomain.Features.TeacherClass.DTO.Command;

namespace TeacherService.Controllers;

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


    [HttpPut("[action]")]
    public async Task<IActionResult> UpdateClass([FromBody] UpdateClassRequest updateClassRequest, CancellationToken token)
        => Ok(await _mediator.Send(new UpdateClassCommand(updateClassRequest), token));

    [HttpPut("[action]")]
    public async Task<IActionResult> ToggleActivateClass([FromBody] int ClassId, CancellationToken token)
         => Ok(await _mediator.Send(new ToggleActivateClassCommand(ClassId), token));


    [HttpPost("[action]")]
    public async Task<IActionResult> RequestToEnrollClass([FromBody] int ClassId, CancellationToken token)
         => Ok(await _mediator.Send(new RequestEnrollToClassCommand(ClassId), token));

    [HttpDelete("[action]")]
    public async Task<IActionResult> UnrollFromClass([FromBody] int ClassId, CancellationToken token)
         => Ok(await _mediator.Send(new UnrollFromClassCommand(ClassId), token));

    [HttpDelete("[action]")]
    public async Task<IActionResult> UnrollFromClassByTeacher([FromBody] UnrollStudentFromClassByTeacherRequest request, CancellationToken token)
         => Ok(await _mediator.Send(new UnrollStudentFromClassByTeacherCommand(request), token));


}
