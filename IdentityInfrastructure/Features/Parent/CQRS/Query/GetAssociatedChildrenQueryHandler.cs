using Flaminco.CommitResult;
using IdentityDomain.Features.Parent.CQRS.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.HttpClients;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;

namespace IdentityInfrastructure.Features.Parent.CQRS.Query
{
    public class GetAssociatedChildrenQueryHandler : IRequestHandler<GetAssociatedChildrenQuery, ICommitResults<LimitedProfileResponse>>
    {
        private readonly STIdentityDbContext _dbContext;
        private readonly Guid? _userId;
        private readonly PaymentClient _paymentClient;

        public GetAssociatedChildrenQueryHandler(STIdentityDbContext dbContext, IHttpContextAccessor httpContextAccessor, PaymentClient paymentClient)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
            _paymentClient = paymentClient;
        }

        public async Task<ICommitResults<LimitedProfileResponse>> Handle(GetAssociatedChildrenQuery Request, CancellationToken cancellationToken)
        {
            IEnumerable<IdentityRelation> relations = await _dbContext.Set<IdentityRelation>()
                               .Where(a => a.PrimaryId.Equals(_userId) && a.RelationType == RelationType.ParentToKid)
                               .Include(a => a.SecondaryFK.GradeFK)
                               .Include(a => a.SecondaryFK.AvatarFK)
                               .ToListAsync(cancellationToken);

            Dictionary<Guid, bool> IsPremiumLookupTable = new Dictionary<Guid, bool>();

            foreach (var item in relations.Select(a => a.SecondaryId))
            {
                IsPremiumLookupTable.Add(item.GetValueOrDefault(), await IsUserPremiumAsync(item, cancellationToken));
            }

            return ResultType.Ok.GetValueCommitResults(relations.Select(a => new LimitedProfileResponse
            {
                UserId = a.SecondaryId.Value,
                FullName = a.SecondaryFK.FullName,
                GradeName = a.SecondaryFK.GradeFK.Name,
                GradeId = a.SecondaryFK.GradeId.Value,
                AvatarImage = a.SecondaryFK.AvatarFK.ImageUrl,
                IsPremium = IsPremiumLookupTable[a.SecondaryId.GetValueOrDefault()]
            }).ToList());
        }

        private async Task<bool> IsUserPremiumAsync(Guid? student, CancellationToken cancellationToken)
        {
            ICommitResult<bool>? result = await _paymentClient.ValidateCurrentUserPaymentStatusAsync(student, null, cancellationToken);
            return result?.IsSuccess == true && result.Value;
        }
    }
}
