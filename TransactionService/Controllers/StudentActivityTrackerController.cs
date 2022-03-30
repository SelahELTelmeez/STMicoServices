using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransactionDomain.Features.Activities.CQRS.Command;
using TransactionDomain.Features.Activities.DTO.Command;

namespace TransactionService.Controllers
{
    [ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StudentActivityTrackerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentActivityTrackerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> InsertStudentActivity([FromBody] ActivityRequestDTO activityrRequest, CancellationToken token)
        => Ok(await _mediator.Send(new ActivityCommand(activityrRequest), token));

    }
}
