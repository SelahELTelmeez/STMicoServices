﻿using Flaminco.CommitResult;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PaymentDomain.Features.GetProductOffers.CQRS.Query;
using PaymentDomain.Features.GetProductOffers.DTO.Query;
using PaymentEntities;
using PaymentEntities.Entities;
using SharedModule.Extensions;

namespace PaymentInfrastructure.Features.GetProductOffers.CQRS.Query;
public class GetProductOffersQueryHandler : IRequestHandler<GetProductOffersQuery, ICommitResult<ProductOfferResponse>>
{
    private readonly PaymentDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly IConfiguration _configuration;
    public GetProductOffersQueryHandler(PaymentDbContext dbContext, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _configuration = configuration;
    }
    public async Task<ICommitResult<ProductOfferResponse>> Handle(GetProductOffersQuery request, CancellationToken cancellationToken)
    {
        // check if the current user is already registed.
        bool alreadyRegisterd = await _dbContext.Set<PurchaseContract>().Where(a => a.UserId.Equals(_userId) && a.TransactionStatus > 1 && a.ExpiredOn >= DateTime.Today).AnyAsync(cancellationToken);
        if (alreadyRegisterd)
        {
            return ResultType.Duplicated.GetValueCommitResult((ProductOfferResponse)null, "X0000");
        }

        if (!string.IsNullOrWhiteSpace(request.Promocode))
        {
            Promocode? promocode = await _dbContext.Set<Promocode>()
                                                   .SingleOrDefaultAsync(a => a.Code.Equals(request.Promocode), cancellationToken);
            if (promocode == null)
            {
                return ResultType.Invalid.GetValueCommitResult<ProductOfferResponse>(default, "X0000");
            }

            return ResultType.Ok.GetValueCommitResult(new ProductOfferResponse
            {
                IsFawryEnabled = bool.Parse(_configuration["PaymentSettings:Fawry:IsEnabled"]),
                IsTPayEnabled = bool.Parse(_configuration["PaymentSettings:TPay:IsEnabled"]),
                MobileOperators = await _dbContext.Set<MobileOperator>().Where(a => a.IsActive).ProjectToType<MobileOperatorResponse>().ToListAsync(cancellationToken),
                ProductResponses = await _dbContext.Set<Product>().Where(a => a.Grade == request.Grade && a.PromotionId.Equals(promocode.PromotionId)).ProjectToType<ProductResponse>().ToListAsync(cancellationToken)
            });
        }
        else
        {
            return ResultType.Ok.GetValueCommitResult(
            new ProductOfferResponse
            {
                IsFawryEnabled = bool.Parse(_configuration["PaymentSettings:Fawry:IsEnabled"]),
                IsTPayEnabled = bool.Parse(_configuration["PaymentSettings:TPay:IsEnabled"]),
                MobileOperators = await _dbContext.Set<MobileOperator>().Where(a => a.IsActive).ProjectToType<MobileOperatorResponse>().ToListAsync(cancellationToken),
                ProductResponses = await _dbContext.Set<Product>().Where(a => a.Grade == request.Grade && a.PromotionId == 1).ProjectToType<ProductResponse>().ToListAsync(cancellationToken)
            });
        }
    }
}