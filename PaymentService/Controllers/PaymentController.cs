using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentDomain.Features.FawryInitializer.CQRS.Command;
using PaymentDomain.Features.FawryInitializer.DTO.Command;
using PaymentDomain.Features.GetProductOffers.CQRS.Query;
using PaymentDomain.Features.GetProductOffers.DTO.Query;
using PaymentDomain.Features.TPay.CQRS.Command;
using PaymentDomain.Features.TPay.DTO.Command;
using ResultHandler;

namespace PaymentService.Controllers;

[Route("api/[controller]"), ApiController, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PaymentController : ControllerBase
{
    private readonly IMediator _mediator;
    public PaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("[action]"), Produces(typeof(CommitResult<ProductOfferResponse>))]
    public async Task<IActionResult> GetProductOffers([FromQuery(Name = "Grade")] int? Grade, [FromQuery(Name = "Promocode")] string? Promocode, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetProductOffersQuery(Grade, Promocode), cancellationToken));

    [HttpPost("[action]"), Produces(typeof(CommitResult<FawryInitializerRespons>))]
    public async Task<IActionResult> GetFawryInitialization(FawryInitializerRequest initializerRequest, CancellationToken cancellationToken)
       => Ok(await _mediator.Send(new FawryInitializerCommand(initializerRequest), cancellationToken));


    [HttpPost("[action]"), Produces(typeof(CommitResult<TPayInitializerResponse>))]
    public async Task<IActionResult> GetTPayInitializerReference(TPayInitializerRequest InitializerRequest, CancellationToken cancellationToken)
       => Ok(await _mediator.Send(new TPayInitializerCommand(InitializerRequest), cancellationToken));

    [HttpPost("[action]"), Produces(typeof(CommitResult<TPayInitializerResponse>))]
    public async Task<IActionResult> TPayConfirmPayment(TPayConfirmPaymentRequest PaymentRequest, CancellationToken cancellationToken)
   => Ok(await _mediator.Send(new TPayConfirmPaymentCommand(PaymentRequest), cancellationToken));


    [HttpGet("[action]"), Produces(typeof(CommitResult<TPayInitializerResponse>))]
    public async Task<IActionResult> TPayResendPinCode([FromQuery(Name = "PurchaseContractId")] int PurchaseContractId, CancellationToken cancellationToken)
   => Ok(await _mediator.Send(new TPayResendPinCodeCommand(PurchaseContractId), cancellationToken));



}
