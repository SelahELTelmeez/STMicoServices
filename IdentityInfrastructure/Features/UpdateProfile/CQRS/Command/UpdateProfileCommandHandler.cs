using IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
using IdentityDomain.Features.UpdateProfile.CQRS.Command;
using IdentityDomain.Features.UpdateProfile.DTO.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ResultHandler;

namespace IdentityInfrastructure.Features.UpdateProfile.CQRS.Command;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, CommitResult<UpdateProfileResponse>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMediator _mediator;

    public UpdateProfileCommandHandler(STIdentityDbContext dbContext,
                                       IWebHostEnvironment configuration,
                                       IHttpContextAccessor httpContextAccessor, IMediator mediator)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _httpContextAccessor = httpContextAccessor;
        _mediator = mediator;
    }
    public async Task<CommitResult<UpdateProfileResponse>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {

        IdentityUser? identityUser = await _mediator.Send(new GetIdentityUserByIdQuery(_httpContextAccessor.GetIdentityUserId()), cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult<UpdateProfileResponse>
            {
                ErrorCode = "X0001",
                ErrorMessage = _resourceJsonManager["X0001"], // facebook data is Exist, try to sign in instead.
                ResultType = ResultType.NotFound,
            };
        }

        // update values.
        identityUser.MobileNumber = request.UpdateProfile.MobileNumber;
        identityUser.AvatarId = request.UpdateProfile.AvatarId;
        identityUser.GovernorateId = request.UpdateProfile.GovernorateId;
        identityUser.Country = (Country)request.UpdateProfile.CountryId.GetValueOrDefault();
        identityUser.DateOfBirth = request.UpdateProfile.DateOfBirth == null ? null : DateTime.Parse(request.UpdateProfile.DateOfBirth);
        identityUser.Gender = (Gender)request.UpdateProfile.Gender.GetValueOrDefault();
        identityUser.IsMobileVerified = request.UpdateProfile.MobileNumber == null ? null : false;
        _dbContext.Set<IdentityUser>().Update(identityUser);

        await _dbContext.SaveChangesAsync(cancellationToken);

        // Loading Related Entities
        await _dbContext.Entry(identityUser).Reference(a => a.AvatarFK).LoadAsync(cancellationToken);
        await _dbContext.Entry(identityUser).Reference(a => a.GradeFK).LoadAsync(cancellationToken);
        await _dbContext.Entry(identityUser).Reference(a => a.IdentityRoleFK).LoadAsync(cancellationToken);
        await _dbContext.Entry(identityUser).Reference(a => a.GovernorateFK).LoadAsync(cancellationToken);

        return new CommitResult<UpdateProfileResponse>
        {
            ResultType = ResultType.Ok,
            Value = new UpdateProfileResponse
            {
                Id = identityUser.Id.ToString(),
                AvatarUrl = $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{Enum.GetName(typeof(AvatarType), identityUser?.AvatarFK?.AvatarType ?? AvatarType.Default)}/{identityUser?.AvatarFK?.ImageUrl ?? "default.png"}",
                Country = Enum.GetName(typeof(Country), identityUser.Country.GetValueOrDefault()),
                Gender = Enum.GetName(typeof(Gender), identityUser.Gender.GetValueOrDefault()),
                DateOfBirth = identityUser.DateOfBirth,
                Email = identityUser.Email,
                FullName = identityUser.FullName,
                Governorate = identityUser.GovernorateFK?.Name,
                Grade = identityUser.GradeFK?.Name,
                IsPremium = identityUser.IsPremium,
                MobileNumber = identityUser.MobileNumber,
                ReferralCode = identityUser.ReferralCode,
                Role = identityUser.IdentityRoleFK?.Name,
                IsEmailVerified = identityUser.IsEmailVerified.GetValueOrDefault(),
                IsMobileVerified = identityUser.IsMobileVerified.GetValueOrDefault()
            }
        };
    }
}


