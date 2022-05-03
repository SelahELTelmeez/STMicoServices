using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentDomain.Features.FawryInitializer.CQRS.Command;
using PaymentDomain.Features.FawryInitializer.DTO.Command;
using PaymentDomain.Features.GetProductOffers.CQRS.Query;
using PaymentDomain.Features.GetProductOffers.DTO.Query;

namespace PaymentService.Controllers
{
    [Route("api/[controller]"), ApiController, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetProductOffers(ProductOfferRequest request, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetProductOffersQuery(request), cancellationToken));

        [HttpPost("[action]")]
        public async Task<IActionResult> FawryeInitialization(FawryInitializerRequest request, CancellationToken cancellationToken)
           => Ok(await _mediator.Send(new FawryInitializerCommand(request), cancellationToken));

    }
}
