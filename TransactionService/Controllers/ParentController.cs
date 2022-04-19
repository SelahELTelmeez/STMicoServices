using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionDomain.Features.Parent.CQRS.Command;
using TransactionDomain.Features.Parent.DTO;

namespace TransactionService.Controllers
{
    [ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ParentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ParentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddParentChild([FromBody] AddParentChildRequest Request, CancellationToken token)
            => Ok(await _mediator.Send(new AddParentChildCommand(Request), token));
    }
}