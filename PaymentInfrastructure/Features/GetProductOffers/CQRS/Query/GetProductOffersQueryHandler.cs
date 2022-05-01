using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PaymentDomain.Features.GetProductOffers.CQRS.Query;
using PaymentDomain.Features.GetProductOffers.DTO.Query;
using PaymentEntities;
using PaymentEntities.Entities;
using SharedModule.Extensions;

namespace PaymentInfrastructure.Features.GetProductOffers.CQRS.Query
{
    public class GetProductOffersQueryHandler : IRequestHandler<GetProductOffersQuery, CommitResult<ProductOfferResponse>>
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
        public async Task<CommitResult<ProductOfferResponse>> Handle(GetProductOffersQuery request, CancellationToken cancellationToken)
        {
            // check if the current user is already registed.
            bool alreadyRegisterd = await _dbContext.Set<PurchaseContract>().Where(a => a.UserId.Equals(_userId) && a.TransactionStatus > 1 && a.ExpiredOn >= DateTime.Today).AnyAsync(cancellationToken);
            if (alreadyRegisterd)
            {
                return new CommitResult<ProductOfferResponse>
                {
                    ErrorCode = "X0000",
                    ErrorMessage = "X0000",
                    ResultType = ResultType.Duplicated
                };
            }



            if (!string.IsNullOrWhiteSpace(request.ProductOfferRequest.Promocode))
            {
                Promocode? promocode = await _dbContext.Set<Promocode>()
                                                       .SingleOrDefaultAsync(a => a.Code.Equals(request.ProductOfferRequest.Promocode), cancellationToken);
                if (promocode == null)
                {
                    return new CommitResult<ProductOfferResponse>
                    {
                        ErrorCode = "X0000",
                        ErrorMessage = "X0000",
                        ResultType = ResultType.Invalid
                    };
                }

                return new CommitResult<ProductOfferResponse>
                {
                    Value = new ProductOfferResponse
                    {
                        IsFawryEnabled = bool.Parse(_configuration["PaymentSettings:Fawry:IsEnabled"]),
                        IsTPayEnabled = bool.Parse(_configuration["PaymentSettings:TPay:IsEnabled"]),
                        MobileOperators = await _dbContext.Set<MobileOperator>().Where(a => a.IsActive).ProjectToType<MobileOperatorResponse>().ToListAsync(cancellationToken),
                        ProductResponses = await _dbContext.Set<Product>().Where(a => a.Grade == request.ProductOfferRequest.Grade && a.PromotionId.Equals(promocode.PromotionId)).ProjectToType<ProductResponse>().ToListAsync(cancellationToken)
                    }
                };
            }
            else
            {
                return new CommitResult<ProductOfferResponse>
                {
                    Value = new ProductOfferResponse
                    {
                        IsFawryEnabled = bool.Parse(_configuration["PaymentSettings:Fawry:IsEnabled"]),
                        IsTPayEnabled = bool.Parse(_configuration["PaymentSettings:TPay:IsEnabled"]),
                        MobileOperators = await _dbContext.Set<MobileOperator>().Where(a => a.IsActive).ProjectToType<MobileOperatorResponse>().ToListAsync(cancellationToken),
                        ProductResponses = await _dbContext.Set<Product>().Where(a => a.Grade == request.ProductOfferRequest.Grade && a.PromotionId == 1).ProjectToType<ProductResponse>().ToListAsync(cancellationToken)
                    }
                };
            }
        }
    }
}
