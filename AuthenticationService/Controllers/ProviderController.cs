using IdentityDomain.Features.GetAvatars.CQRS.Query;
using IdentityDomain.Features.GradesDropDown.CQRS.Query;
using IdentityDomain.Features.IdentityGrade.CQRS.Query;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProviderController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProviderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("[action]"), AllowAnonymous]
        public async Task<IActionResult> GetGradesDropDownMenu(CancellationToken token)
             => Ok(await _mediator.Send(new GetGradesDropDownMenuQuery(), token));

        [HttpGet("[action]"), AllowAnonymous]
        public async Task<IActionResult> GetAvatars(CancellationToken token)
            => Ok(await _mediator.Send(new GetAvatarsQuery(), token));


        [HttpGet("[action]")]
        public async Task<IActionResult> GetIdentityGrade(CancellationToken token)
              => Ok(await _mediator.Send(new GetIdentityGradeQuery(), token));
    }
}
