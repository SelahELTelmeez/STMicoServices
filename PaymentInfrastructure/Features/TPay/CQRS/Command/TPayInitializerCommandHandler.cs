﻿using Flaminco.CommitResult;
using Flaminco.JsonLocalizer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PaymentDomain.Features.TPay.CQRS.Command;
using PaymentDomain.Features.TPay.DTO.Command;
using PaymentEntities;
using PaymentEntities.Entities;
using PaymentInfrastructure.HttpClients;
using SharedModule.Extensions;
using SharedModule.Utilities;

namespace PaymentInfrastructure.Features.TPay.CQRS.Command;
public class TPayInitializerCommandHandler : IRequestHandler<TPayInitializerCommand, ICommitResult<int>>
{
    private readonly TPayClient _TPayClient;
    private readonly PaymentDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly JsonLocalizerManager _resourceJsonManager;
    public TPayInitializerCommandHandler(TPayClient tPayClient, IHttpContextAccessor httpContextAccessor, PaymentDbContext dbContext, JsonLocalizerManager resourceJsonManager)
    {
        _TPayClient = tPayClient;
        _userId = httpContextAccessor.GetIdentityUserId();
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
    }
    public async Task<ICommitResult<int>> Handle(TPayInitializerCommand request, CancellationToken cancellationToken)
    {
        Product? product = await _dbContext.Set<Product>().SingleOrDefaultAsync(a => a.Id.Equals(request.PayInitializerRequest.ProductId), cancellationToken);
        if (product == null)
        {
            return ResultType.NotFound.GetValueCommitResult<int>(default, "X0002", _resourceJsonManager["X0002"]);
        }

        ICommitResult<TPayInitializerResponse> commitResult = await _TPayClient.InitializerAsync(request.PayInitializerRequest, product, cancellationToken);
        if (!commitResult.IsSuccess)
        {
            return ResultType.Invalid.GetValueCommitResult<int>(default, commitResult.ErrorCode, commitResult.ErrorMessage);
        }
        if (commitResult.Value.OperationStatusCode == 10) // everything is okay
        {
            EntityEntry<PurchaseContract> purchaseContract = _dbContext.Set<PurchaseContract>().Add(new PurchaseContract
            {
                UserId = _userId.GetValueOrDefault(),
                PlanType = "OneTimePayment",
                TransactionId = commitResult.Value.TransactionId,
                TransactionStatus = 0,
                ExpiredOn = DateTime.Today.AddDays(product.SubscriptionDurationInDays),
                ProductId = request.PayInitializerRequest.ProductId
            });

            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetValueCommitResult(purchaseContract.Entity.Id);
        }
        else
        {
            return ResultType.Invalid.GetValueCommitResult<int>(default, "X0000", errorMessage: CodeMappers.TPayCodeMapper(commitResult.Value.OperationStatusCode, commitResult.Value.ErrorMessage));
        }
    }
}