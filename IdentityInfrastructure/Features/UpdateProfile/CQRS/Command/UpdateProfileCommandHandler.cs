using IdentityDomain.Features.UpdateProfile.CQRS.Command;
using IdentityDomain.Features.UpdateProfile.DTO.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using JWTGenerator.TokenHandler;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.UpdateProfile.CQRS.Command;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, CommitResult<UpdateProfileResponseDTO>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateProfileCommandHandler(STIdentityDbContext dbContext, JsonLocalizerManager resourceJsonManager, TokenHandlerManager tokenHandlerManager, IHttpContextAccessor httpContextAccessor = null)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<CommitResult<UpdateProfileResponseDTO>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {

        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Id.Equals(HttpIdentityUser.GetIdentityUserId(_httpContextAccessor)), cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult<UpdateProfileResponseDTO>
            {
                ErrorCode = "X0004",
                ErrorMessage = _resourceJsonManager["X0004"], // facebook data is Exist, try to sign in instead.
                ResultType = ResultType.NotFound,
            };
        }

        // update values.
        identityUser.AvatarId = request.UpdateProfile.AvatarId;
        identityUser.GovernorateId = request.UpdateProfile.GovernorateId;
        identityUser.Country = (Country)request.UpdateProfile.CountryId.GetValueOrDefault();
        identityUser.DateOfBirth = request.UpdateProfile.DateOfBirth == null ? null : DateTime.Parse(request.UpdateProfile.DateOfBirth);
        identityUser.Gender = (Gender)request.UpdateProfile.Gender.GetValueOrDefault();

        _dbContext.Set<IdentityUser>().Update(identityUser);

        await _dbContext.SaveChangesAsync(cancellationToken);

        // Loading Related Entities
        await _dbContext.Entry(identityUser).Reference(a => a.AvatarFK).LoadAsync(cancellationToken);
        await _dbContext.Entry(identityUser).Reference(a => a.GradeFK).LoadAsync(cancellationToken);
        await _dbContext.Entry(identityUser).Reference(a => a.IdentityRoleFK).LoadAsync(cancellationToken);
        await _dbContext.Entry(identityUser).Reference(a => a.GovernorateFK).LoadAsync(cancellationToken);

        return new CommitResult<UpdateProfileResponseDTO>
        {
            ResultType = ResultType.Ok,
            Value = new UpdateProfileResponseDTO
            {
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
            }
        };
    }
}


