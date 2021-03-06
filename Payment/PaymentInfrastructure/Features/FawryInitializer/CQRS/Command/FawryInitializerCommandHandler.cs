using Flaminco.CommitResult;
using Flaminco.JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using PaymentDomain.Features.FawryInitializer.CQRS.Command;
using PaymentDomain.Features.FawryInitializer.DTO.Command;
using PaymentEntities;
using PaymentEntities.Entities;
using SharedModule.Extensions;

namespace PaymentInfrastructure.Features.FawryInitializer.CQRS.Command;

public class FawryInitializerCommandHandler : IRequestHandler<FawryInitializerCommand, ICommitResult<FawryInitializerRespons>>
{
    private readonly PaymentDbContext _dbContext;
    private readonly string? _userId;
    private readonly IConfiguration _configuration;
    private readonly JsonLocalizerManager _resourceJsonManager;
    public FawryInitializerCommandHandler(PaymentDbContext dbContext, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _configuration = configuration;
        _resourceJsonManager = new JsonLocalizerManager(httpContextAccessor, webHostEnvironment);
    }


    public async Task<ICommitResult<FawryInitializerRespons>> Handle(FawryInitializerCommand request, CancellationToken cancellationToken)
    {
        Product? product = await _dbContext.Set<Product>().FirstOrDefaultAsync(a => a.Id.Equals(request.FawryInitializerRequest.ProductId), cancellationToken);

        if (product == null)
        {
            return ResultType.NotFound.GetValueCommitResult<FawryInitializerRespons>(default, "XPYM0002", _resourceJsonManager["XPYM0002"]);
        }

        EntityEntry<PurchaseContract> purchaseContract = _dbContext.Set<PurchaseContract>().Add(new PurchaseContract
        {
            UserId = _userId,
            PlanType = "FawryPayment",
            TransactionId = Guid.NewGuid().ToString(),
            TransactionStatus = 0,
            ExpiredOn = DateTime.Today.AddDays(product.SubscriptionDurationInDays),
            ProductId = product.Id,
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        FawryInitializerRespons initializerRespons = new FawryInitializerRespons
        {
            MerchantCode = _configuration["PaymentSettings:Fawry:MerchantCode"],
            MerchantRefNum = purchaseContract.Entity.TransactionId,
            CustomerEmail = request.FawryInitializerRequest.EmailAddress,
            CustomerMobile = request.FawryInitializerRequest.MobileNumber,
            CustomerName = request.FawryInitializerRequest.CustomerName,
            CustomerProfileId = request.FawryInitializerRequest.UserId,
            Image = string.Empty,
            ItemDescription = string.Empty,
            ItemId = product.Id,
            Language = "ar",
            PaymentMethod = string.Empty,
            Price = string.Format("{0:0.00}", product.Price),
            Quantity = 1,
            ReturnUrl = _configuration["PaymentSettings:Fawry:ReturnUrl"],
        };

        AssignInitializerSignature(initializerRespons);

        return ResultType.Ok.GetValueCommitResult(initializerRespons);
    }
    private void AssignInitializerSignature(FawryInitializerRespons initializerRespons)
    {
        string contentToHash = string.Format("{0}{1}{2}{3}{4}{5}{6}",
            initializerRespons.MerchantCode,
            initializerRespons.MerchantRefNum,
            initializerRespons.CustomerProfileId,
            initializerRespons.ReturnUrl,
            initializerRespons.ItemId,
            initializerRespons.Price,
            _configuration["PaymentSettings:Fawry:SecurityKey"]
            );
        var hash = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(_configuration["PaymentSettings:TPay:PrivateKey"]));
        initializerRespons.Signature = string.Join(string.Empty, hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(contentToHash)).Select(b => b.ToString("x2")));
    }

}
