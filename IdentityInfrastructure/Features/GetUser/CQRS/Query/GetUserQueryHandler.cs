using IdentityDomain.Features.GetUser.CQRS.Query;
using IdentityDomain.Features.Login.DTO.Command;
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
using Microsoft.IdentityModel.JsonWebTokens;
using ResultHandler;

namespace IdentityInfrastructure.Features.GetUser.CQRS.Query;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, CommitResult<LoginResponse>>
{
    private readonly STIdentityDbContext _dbContext;
    private Guid? _userId;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly TokenHandlerManager _jwtAccessGenerator;
    private readonly PaymentClient _paymentClient;

    public GetUserQueryHandler(STIdentityDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment configuration,
        TokenHandlerManager tokenHandlerManager,
        PaymentClient paymentClient)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _jwtAccessGenerator = tokenHandlerManager;
        _paymentClient = paymentClient;
    }
    public async Task<CommitResult<LoginResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        IdentityUser? user = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Id == _userId);
        if (user == null)
        {
            return new CommitResult<LoginResponse>
            {
                ResultType = ResultType.NotFound,
                ErrorCode = "XIDN0001",
                ErrorMessage = _resourceJsonManager["XIDN0001"]
            };
        }
        return new CommitResult<LoginResponse>
        {
            ResultType = ResultType.Ok,
            Value = await LoadRelatedEntitiesAsync(user, false, cancellationToken)
        };
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


        CommitResult<bool>? validateSubscription = await _paymentClient.ValidateCurrentUserPaymentStatusAsync(identityUser.Id, cancellationToken);

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
        }; ;
    }
}

