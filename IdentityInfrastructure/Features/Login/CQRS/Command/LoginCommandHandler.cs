using IdentityDomain.Features.Login.CQRS.Command;
using IdentityDomain.Features.Login.DTO.Command;
using IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.IdentityModel.Tokens.Jwt;
using DomainEntities = IdentityEntities.Entities.Identities;

namespace IdentityInfrastructure.Features.Login.CQRS.Command;
public class LoginCommandHandler : IRequestHandler<LoginCommand, CommitResult<LoginResponse>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly TokenHandlerManager _jwtAccessGenerator;
    private readonly IMediator _mediator;

    public LoginCommandHandler(STIdentityDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment configuration,
        TokenHandlerManager tokenHandlerManager, IMediator mediator)
    {

        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _jwtAccessGenerator = tokenHandlerManager;
        _mediator = mediator;
    }
    public async Task<CommitResult<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
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
            return await GetExternalProviderAsync(request.LoginRequest.OfficeId, "Office", cancellationToken);
        }
        //2. Check if the user exists with basic data entry.

        // Check by email first.
        IdentityUser? identityUser = default;
        if (!string.IsNullOrEmpty(request.LoginRequest.Email))
        {
            identityUser = await _mediator.Send(new GetIdentityUserByEmailAndPasswordQuery(request.LoginRequest.Email, request.LoginRequest.PasswordHash.Encrypt(true)), cancellationToken);
        }
        // check by mobile number
        if (!string.IsNullOrEmpty(request.LoginRequest.MobileNumber))
        {
            identityUser = await _mediator.Send(new GetIdentityUserByMobileAndPasswordQuery(request.LoginRequest.MobileNumber, request.LoginRequest.PasswordHash.Encrypt(true)), cancellationToken);

        }
        if (identityUser == null)
        {
            return new CommitResult<LoginResponse>
            {
                ErrorCode = "X0001",
                ErrorMessage = _resourceJsonManager["X0001"],
                ResultType = ResultType.NotFound,
            };
        }
        return new CommitResult<LoginResponse>
        {
            ResultType = ResultType.Ok,
            Value = await LoadRelatedEntitiesAsync(identityUser, false, cancellationToken)
        };
    }
    private async Task<CommitResult<LoginResponse>> GetExternalProviderAsync(string providerId, string providerName, CancellationToken cancellationToken)
    {
        DomainEntities.ExternalIdentityProvider? externalIdentityProvider = await _dbContext.Set<DomainEntities.ExternalIdentityProvider>()
            .Include(a => a.IdentityUserFK)
            .SingleOrDefaultAsync(a => a.Name == providerName && a.Identifierkey == providerId, cancellationToken);

        if (externalIdentityProvider == null)
        {
            return new CommitResult<LoginResponse>
            {
                ErrorCode = "X0005",
                ErrorMessage = _resourceJsonManager["X0005"],
                ResultType = ResultType.NotFound,
            };
        }
        else
        {
            // Loading related data.
            return new CommitResult<LoginResponse>
            {
                ResultType = ResultType.Ok,
                Value = await LoadRelatedEntitiesAsync(externalIdentityProvider.IdentityUserFK, true, cancellationToken)
            };
        }
    }
    private async Task<LoginResponse> LoadRelatedEntitiesAsync(IdentityUser identityUser, bool isExternal, CancellationToken cancellationToken)
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
        foreach (IdentityRefreshToken token in identityUser.RefreshTokens.Where(a => a.IsActive && a.Token != identityRefreshToken.Token))
            token.RevokedOn = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);


        // Mapping To return the result to the User.

        return new LoginResponse
        {
            Id = identityUser.Id.ToString(),
            FullName = identityUser.FullName,
            AccessToken = accessToken.Token,
            RefreshToken = refreshToken.Token,
            ReferralCode = identityUser.ReferralCode,
            AvatarUrl = $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{identityUser.AvatarFK?.AvatarType ?? "Default"}/{identityUser.AvatarFK?.ImageUrl ?? "default.png"}",
            DateOfBirth = identityUser.DateOfBirth,
            Email = identityUser.Email,
            MobileNumber = identityUser.MobileNumber,
            Governorate = identityUser?.GovernorateFK?.Name,
            Grade = identityUser?.GradeFK?.Name,
            IsPremium = identityUser.IsPremium,
            Role = identityUser?.IdentityRoleFK?.Name,
            Country = Enum.GetName(typeof(Country), identityUser?.Country.GetValueOrDefault()),
            Gender = Enum.GetName(typeof(Gender), identityUser?.Gender.GetValueOrDefault()),
            IsEmailVerified = isExternal ? true : identityUser.IsEmailVerified.GetValueOrDefault(),
            IsMobileVerified = isExternal ? true : identityUser.IsMobileVerified.GetValueOrDefault(),
        }; ;
    }
}