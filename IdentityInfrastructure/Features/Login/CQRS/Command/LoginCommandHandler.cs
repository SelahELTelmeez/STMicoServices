using IdentityDomain.Features.Login.CQRS.Command;
using IdentityDomain.Features.Login.DTO.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.IdentityModel.Tokens.Jwt;
using DomainEntities = IdentityEntities.Entities.Identities;

namespace IdentityInfrastructure.Features.Login.CQRS.Command;
public class LoginCommandHandler : IRequestHandler<LoginCommand, CommitResult<LoginResponseDTO>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly TokenHandlerManager _jwtAccessGenerator;
    public LoginCommandHandler(STIdentityDbContext dbContext, JsonLocalizerManager resourceJsonManager, TokenHandlerManager tokenHandlerManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
        _jwtAccessGenerator = tokenHandlerManager;
    }
    public async Task<CommitResult<LoginResponseDTO>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        //1. Access the database to check of the existence of the user with different providers.
        if (!string.IsNullOrWhiteSpace(request.LoginRequest.GoogleId))
        {
            return await GetExternalProviderAsync(request.LoginRequest.GoogleId, "Google", cancellationToken);
        }
        if (!string.IsNullOrWhiteSpace(request.LoginRequest.FacebookId))
        {
            return await GetExternalProviderAsync(request.LoginRequest.FacebookId, "Facebook", cancellationToken);
        }
        if (!string.IsNullOrWhiteSpace(request.LoginRequest.OfficeId))
        {
            return await GetExternalProviderAsync(request.LoginRequest.FacebookId, "Office", cancellationToken);
        }
        //2. Check if the user exists with basic data entry.

        // Check by email first.
        IdentityUser? identityUser = default;
        if (!string.IsNullOrEmpty(request.LoginRequest.Email))
        {
            identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Email.Equals(request.LoginRequest.Email) && a.PasswordHash == request.LoginRequest.PasswordHash);
        }
        // check by mobile number
        if (!string.IsNullOrEmpty(request.LoginRequest.MobileNumber))
        {
            identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.MobileNumber == request.LoginRequest.MobileNumber && a.PasswordHash == request.LoginRequest.PasswordHash);
        }
        if (identityUser == null)
        {
            return new CommitResult<LoginResponseDTO>
            {
                ErrorCode = "X0001",
                ErrorMessage = _resourceJsonManager["X0001"],
                ResultType = ResultType.NotFound,
            };
        }
        return new CommitResult<LoginResponseDTO>
        {
            ResultType = ResultType.Ok,
            Value = await LoadRelatedEntitiesAsync(identityUser, cancellationToken)
        };
    }
    private async Task<CommitResult<LoginResponseDTO>> GetExternalProviderAsync(string providerId, string providerName, CancellationToken cancellationToken)
    {
        DomainEntities.ExternalIdentityProvider? externalIdentityProvider = await _dbContext.Set<DomainEntities.ExternalIdentityProvider>()
            .Include(a => a.IdentityUserFK)
            .SingleOrDefaultAsync(a => a.Name == providerName && a.ProviderId == providerId, cancellationToken);

        if (externalIdentityProvider == null)
        {
            return new CommitResult<LoginResponseDTO>
            {
                ErrorCode = "X0003",
                ErrorMessage = _resourceJsonManager["X0003"],
                ResultType = ResultType.NotFound,
            };
        }
        else
        {
            // Loading related data.
            return new CommitResult<LoginResponseDTO>
            {
                ResultType = ResultType.Ok,
                Value = await LoadRelatedEntitiesAsync(externalIdentityProvider.IdentityUserFK, cancellationToken)
            };
        }
    }
    private async Task<LoginResponseDTO> LoadRelatedEntitiesAsync(IdentityUser identityUser, CancellationToken cancellationToken)
    {
        // Loading Related Entities
        await _dbContext.Entry(identityUser).Collection(a => a.RefreshTokens).LoadAsync(cancellationToken);
        await _dbContext.Entry(identityUser).Reference(a => a.AvatarFK).LoadAsync(cancellationToken);
        await _dbContext.Entry(identityUser).Reference(a => a.GradeFK).LoadAsync(cancellationToken);
        await _dbContext.Entry(identityUser).Reference(a => a.IdentityRoleFK).LoadAsync(cancellationToken);
        await _dbContext.Entry(identityUser).Reference(a => a.GovernorateFK).LoadAsync(cancellationToken);
        await _dbContext.Entry(identityUser).Collection(a => a.Activations).LoadAsync(cancellationToken);

        // Generate Both Access and Refresh Tokens
        AccessToken accessToken = _jwtAccessGenerator.GetAccessToken(new Dictionary<string, string>()
        {
            {JwtRegisteredClaimNames.Sub, identityUser.Id.ToString()},
        });
        RefreshToken refreshToken = _jwtAccessGenerator.GetRefreshToken();

        // Save Refresh Token into Database.
        IdentityRefreshToken identityRefreshToken = refreshToken.Adapt<IdentityRefreshToken>();
        identityRefreshToken.IdentityUserId = identityUser.Id;
        _dbContext.Set<IdentityRefreshToken>().Add(identityRefreshToken);

        // disable all pre. active refreshTokens
        foreach (IdentityRefreshToken token in identityUser.RefreshTokens.Where(a => a.IsActive))
            token.RevokedOn = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);


        // Mapping To return the result to the User.

        return new LoginResponseDTO
        {
            FullName = identityUser.FullName,
            AccessToken = accessToken.Token,
            RefreshToken = refreshToken.Token,
            ReferralCode = identityUser.ReferralCode,
            AvatarUrl = $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{Enum.GetName(typeof(AvatarType), identityUser.AvatarFK?.AvatarType ?? AvatarType.Default)}/{identityUser.AvatarFK?.ImageUrl ?? "default.png"}",
            DateOfBirth = identityUser.DateOfBirth,
            Email = identityUser.Email,
            MobileNumber = identityUser.MobileNumber,
            Governorate = identityUser?.GovernorateFK?.Name,
            Grade = identityUser?.GradeFK?.Name,
            IsPremium = identityUser.IsPremium,
            Role = identityUser?.IdentityRoleFK?.Name,
            Country = Enum.GetName(typeof(Country), identityUser?.Country.GetValueOrDefault()),
            Gender = Enum.GetName(typeof(Gender), identityUser?.Gender.GetValueOrDefault()),
            IsVerified = identityUser.Activations.OrderByDescending(a => a.CreatedOn).Take(1).Any(a => a.IsVerified)
        }; ;
    }
}