using Flaminco.CommitResult;
using Flaminco.JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PaymentDomain.Features.Contract.Query;
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
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IMediator _mediator;
    public GetProductOffersQueryHandler(PaymentDbContext dbContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, IMediator mediator)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _configuration = configuration;
        _resourceJsonManager = new JsonLocalizerManager(httpContextAccessor, webHostEnvironment);
        _mediator = mediator;
    }
    public async Task<ICommitResult<ProductOfferResponse>> Handle(GetProductOffersQuery request, CancellationToken cancellationToken)
    {
        // check if the current user is already registed.

        ICommitResult<bool> alreadyRegisteredResult = await _mediator.Send(new ValidateCurrentPurchaseContractQuery(_userId));

        if (!alreadyRegisteredResult.IsSuccess || !alreadyRegisteredResult.Value)
        {
            return ResultType.Duplicated.GetValueCommitResult<ProductOfferResponse>(default, "XPYM0003", _resourceJsonManager["XPYM0003"]);
        }

        if (!string.IsNullOrWhiteSpace(request.Promocode))
        {
            Promocode? promocode = await _dbContext.Set<Promocode>()
                                                   .FirstOrDefaultAsync(a => a.Code.Equals(request.Promocode), cancellationToken);
            if (promocode == null)
            {
                return ResultType.Invalid.GetValueCommitResult<ProductOfferResponse>(default, "XPYM0004", _resourceJsonManager["XPYM0004"]);
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