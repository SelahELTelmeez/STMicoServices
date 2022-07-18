using Flaminco.CommitResult;
using Flaminco.JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PaymentDomain.Features.Contract.CQRS.Command;
using PaymentEntities;
using PaymentEntities.Entities;
using SharedModule.Extensions;

namespace PaymentInfrastructure.Features.Contract.CQRS.Command;

public class UnsubscribeAccountCommandHandler : IRequestHandler<UnsubscribeAccountCommand, ICommitResult>
{
    private readonly PaymentDbContext _dbContext;
    private readonly string? _userId;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public UnsubscribeAccountCommandHandler(PaymentDbContext dbContext, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _resourceJsonManager = new JsonLocalizerManager(httpContextAccessor, webHostEnvironment);
    }
    public async Task<ICommitResult> Handle(UnsubscribeAccountCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<PurchaseContract>? purchaseContracts = await _dbContext.Set<PurchaseContract>().Where(a => a.UserId == _userId).ToListAsync(cancellationToken);

        if (!purchaseContracts.Any())
        {
            return ResultType.Empty.GetCommitResult("XPYM0001", _resourceJsonManager["XPYM0001"]);
        }

        PurchaseContract? purchaseContract = purchaseContracts.Where(a => DateTime.UtcNow.InRange(a.CreatedOn.Value, a.ExpiredOn)).FirstOrDefault();

        if (purchaseContract == null)
        {
            return ResultType.Empty.GetCommitResult("XPYM0001", _resourceJsonManager["XPYM0001"]);
        }
        else
        {
            _dbContext.Set<PurchaseContract>().Remove(purchaseContract);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        return ResultType.Ok.GetCommitResult();
    }
}


