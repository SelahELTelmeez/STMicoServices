using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentDomain.Features.FawryInitializer.CQRS.Command;
using PaymentDomain.Features.FawryInitializer.DTO.Command;
using PaymentDomain.Features.GetProductOffers.CQRS.Query;
using PaymentDomain.Features.GetProductOffers.DTO.Query;

namespace PaymentService.Controllers
{
    [Route("api/[controller]"), ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<IActionResult> GetProductOffers(ProductOfferRequest request, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetProductOffersQuery(request), cancellationToken));

        public async Task<IActionResult> FawryeInitialization(FawryInitializerRequest request, CancellationToken cancellationToken)
           => Ok(await _mediator.Send(new FawryInitializerCommand(request), cancellationToken));

    }
}
