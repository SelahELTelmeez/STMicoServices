﻿using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentDomain.Features.FawryInitializer.CQRS.Command;
using PaymentDomain.Features.GetProductOffers.CQRS.Query;
using PaymentDomain.Features.GetProductOffers.DTO.Query;
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

    [HttpGet("[action]"), Produces(typeof(CommitResult<Guid>))]
    public async Task<IActionResult> GetFawryInitializationReference([FromQuery(Name = "Grade")] int? Grade, [FromQuery(Name = "ProductId")] int? ProductId, CancellationToken cancellationToken)
       => Ok(await _mediator.Send(new FawryInitializerCommand(Grade, ProductId), cancellationToken));

}
