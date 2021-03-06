using Flaminco.CommitResult;
using IdentityDomain.Features.Login.CQRS.Command;
using IdentityDomain.Features.Login.DTO.Command;
using IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.HttpClients;
using JsonLocalizer;
using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using DomainEntities = IdentityEntities.Entities.Identities;

namespace IdentityInfrastructure.Features.Login.CQRS.Command;
public class LoginCommandHandler : IRequestHandler<LoginCommand, ICommitResult<LoginResponse>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly TokenHandlerManager _jwtAccessGenerator;
    private readonly IMediator _mediator;
    private readonly PaymentClient paymentClient;

    public LoginCommandHandler(STIdentityDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment configuration,
        TokenHandlerManager tokenHandlerManager, IMediator mediator, PaymentClient paymentClient)
    {

        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _jwtAccessGenerator = tokenHandlerManager;
        _mediator = mediator;
        this.paymentClient = paymentClient;
    }
    public async Task<ICommitResult<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
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
            identityUser = await _mediator.Send(new GetIdentityUserByEmailAndPasswordQuery(request.LoginRequest.Email?.Trim()?.ToLower(), request.LoginRequest.PasswordHash.Encrypt(true)), cancellationToken);
        }
        // check by mobile number
        if (!string.IsNullOrEmpty(request.LoginRequest.MobileNumber))
        {
            identityUser = await _mediator.Send(new GetIdentityUserByMobileAndPasswordQuery(request.LoginRequest.MobileNumber, request.LoginRequest.PasswordHash.Encrypt(true)), cancellationToken);

        }
        if (identityUser == null)
        {
            return ResultType.NotFound.GetValueCommitResult((LoginResponse)null, "XIDN0001", _resourceJsonManager["XIDN0001"]);
        }
        return ResultType.Ok.GetValueCommitResult(await LoadRelatedEntitiesAsync(identityUser, false, cancellationToken));
    }
    private async Task<ICommitResult<LoginResponse>> GetExternalProviderAsync(string providerId, string providerName, CancellationToken cancellationToken)
    {
        DomainEntities.ExternalIdentityProvider? externalIdentityProvider = await _dbContext.Set<DomainEntities.ExternalIdentityProvider>()
            .Include(a => a.IdentityUserFK)
            .FirstOrDefaultAsync(a => a.Name == providerName && a.Identifierkey == providerId, cancellationToken);

        if (externalIdentityProvider == null)
        {
            return ResultType.NotFound.GetValueCommitResult((LoginResponse)null, "XIDN0016", _resourceJsonManager["XIDN0016"]);
        }
        else
        {
            // Loading related data.
            return ResultType.Ok.GetValueCommitResult(await LoadRelatedEntitiesAsync(externalIdentityProvider.IdentityUserFK, true, cancellationToken));
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

        ICommitResult<bool>? validateSubscription = await paymentClient.ValidateCurrentUserPaymentStatusAsync(identityUser.Id, accessToken.Token, cancellationToken);
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
            IsPremium = validateSubscription.IsSuccess == true && (validateSubscription?.Value ?? false),
            Role = identityUser?.IdentityRoleFK?.Name,
            Country = Enum.GetName(typeof(Country), identityUser?.Country.GetValueOrDefault()),
            Gender = Enum.GetName(typeof(Gender), identityUser?.Gender.GetValueOrDefault()),
            IsEmailVerified = isExternal || identityUser.IsEmailVerified.GetValueOrDefault(),
            IsMobileVerified = isExternal || identityUser.IsMobileVerified.GetValueOrDefault(),
            GradeId = identityUser.GradeId,
            RoleId = identityUser.IdentityRoleId
        };
    }
}