using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.Parent.CQRS.Command;

public class AddNewChildAccountCommandHandler : IRequestHandler<AddChildAccountCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly Guid? _userId;

    public AddNewChildAccountCommandHandler(STIdentityDbContext dbContext,
                                  IWebHostEnvironment configuration,
                                  IHttpContextAccessor httpContextAccessor)

    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _userId = httpContextAccessor.GetIdentityUserId();
    }

    public async Task<CommitResult> Handle(AddChildAccountCommand request, CancellationToken cancellationToken)
    {

        // 1.0 Check for the child already added to this parent
        // check for duplicated data.
        IdentityRelation? identityUser = await _dbContext.Set<IdentityRelation>().FirstOrDefaultAsync(a => a.RelationType == RelationType.ParentToKid
                                                                                                         && a.PrimaryId.Equals(_userId)
                                                                                                         && a.SecondaryFK.FullName.Equals(request.AddNewChildRequest.FullName), cancellationToken);
        if (identityUser != null)
        {
            // in case of the duplicated data is not validated, then delete the old ones.
            return new CommitResult
            {
                ResultType = ResultType.Duplicated,
                ErrorCode = "XIDN0018",
                ErrorMessage = _resourceJsonManager["XIDN0018"]
            };
        }

        //2.0 Start Adding the user to the databse.
        IdentityUser user = new IdentityUser
        {
            FullName = request.AddNewChildRequest.FullName,
            DateOfBirth = request.AddNewChildRequest.DateOfBirth,
            Gender = request.AddNewChildRequest.Gender,
            //ExternalIdentityProviders = request.RegisterRequest.GetExternalProviders(),
            ReferralCode = UtilityGenerator.GetUniqueDigits(),
            GradeId = request.AddNewChildRequest.GradeId,
            AvatarId = 0,
            IsPremium = false,
            IdentityRoleId = 1, //student
        };

        _dbContext.Set<IdentityUser>().Add(user);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}
