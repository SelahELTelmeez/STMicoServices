using IdentityDomain.Features.Login.CQRS.Command;
using IdentityDomain.Features.Login.DTO.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using JWTGenerator.TokenHandler;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.IdentityModel.Tokens.Jwt;
using DomainEntities = IdentityEntities.Entities.Identities;

namespace IdentityInfrastructure.Features.Login.CQRS.Command;
public class LoginCommandHandler : IRequestHandler<LoginCommand, CommitResult<LoginResponseDTO>>
{
    private readonly AuthenticationDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly TokenHandlerManager _jwtAccessGenerator;
    public LoginCommandHandler(AuthenticationDbContext dbContext, JsonLocalizerManager resourceJsonManager, TokenHandlerManager tokenHandlerManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
        _jwtAccessGenerator = tokenHandlerManager;
    }
    public async Task<CommitResult<LoginResponseDTO>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        //1. Access the database to check of the existence of the user with different providers.
        if (!string.IsNullOrWhiteSpace(request.IdentityLoginRequest.GoogleId))
        {
            return await GetExternalProviderAsync(request.IdentityLoginRequest.GoogleId, "Google", cancellationToken);
        }
        if (!string.IsNullOrWhiteSpace(request.IdentityLoginRequest.FacebookId))
        {
            return await GetExternalProviderAsync(request.IdentityLoginRequest.FacebookId, "Facebook", cancellationToken);
        }
        if (!string.IsNullOrWhiteSpace(request.IdentityLoginRequest.OfficeId))
        {
            return await GetExternalProviderAsync(request.IdentityLoginRequest.FacebookId, "Office", cancellationToken);
        }
        //2. Check if the user exists with basic data entry.
        // Check by email first.
        if (!string.IsNullOrEmpty(request.IdentityLoginRequest.Email))
        {
            IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Email.Equals(request.IdentityLoginRequest.Email, StringComparison.OrdinalIgnoreCase) && a.PasswordHash == request.IdentityLoginRequest.PasswordHash);
            if (identityUser == null)
            {
                return new CommitResult<LoginResponseDTO>
                {
                    ErrorCode = "X0002",
                    ErrorMessage = _resourceJsonManager["X0002"],
                    ResultType = ResultType.NotFound,
                };
            }
            else
            {
                return new CommitResult<LoginResponseDTO>
                {
                    ResultType = ResultType.Ok,
                    Value = identityUser.Adapt<LoginResponseDTO>()
                };
            }
        }
        // check by mobile number
        if (!string.IsNullOrEmpty(request.IdentityLoginRequest.MobileNumber))
        {
            IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.MobileNumber == request.IdentityLoginRequest.MobileNumber && a.PasswordHash == request.IdentityLoginRequest.PasswordHash);
            if (identityUser == null)
            {
                return new CommitResult<LoginResponseDTO>
                {
                    ErrorCode = "X0002",
                    ErrorMessage = _resourceJsonManager["X0002"],
                    ResultType = ResultType.NotFound,
                };
            }
            else
            {
                return new CommitResult<LoginResponseDTO>
                {
                    ResultType = ResultType.Ok,
                    Value = identityUser.Adapt<LoginResponseDTO>()
                };
            }
        }
        return new CommitResult<LoginResponseDTO>
        {
            ErrorCode = "X0003",
            ErrorMessage = _resourceJsonManager["X0003"],
            ResultType = ResultType.Invalid,
        };
    }
    private async Task<CommitResult<LoginResponseDTO>> GetExternalProviderAsync(string providerId, string providerName, CancellationToken cancellationToken)
    {
        DomainEntities.ExternalIdentityProvider? externalIdentityProvider = await _dbContext.Set<DomainEntities.ExternalIdentityProvider>().SingleOrDefaultAsync(a => a.Name == providerName && a.ProviderId == providerId, cancellationToken);
        if (externalIdentityProvider == null)
        {
            return new CommitResult<LoginResponseDTO>
            {
                ErrorCode = "X0001",
                ErrorMessage = _resourceJsonManager["X0001"],
                ResultType = ResultType.NotFound,
            };
        }
        else
        {
            // Loading related data.
            await _dbContext.Entry(externalIdentityProvider).Reference(a => a.IdentityUserFK).LoadAsync(cancellationToken);
            await _dbContext.Entry(externalIdentityProvider.IdentityUserFK).Reference(a => a.AvatarFK).LoadAsync(cancellationToken);
            await _dbContext.Entry(externalIdentityProvider.IdentityUserFK).Reference(a => a.GradeFK).LoadAsync(cancellationToken);
            await _dbContext.Entry(externalIdentityProvider.IdentityUserFK).Reference(a => a.IdentityRoleFK).LoadAsync(cancellationToken);
            await _dbContext.Entry(externalIdentityProvider.IdentityUserFK).Reference(a => a.GovernorateFK).LoadAsync(cancellationToken);
            LoginResponseDTO responseDTO = externalIdentityProvider.IdentityUserFK.Adapt<LoginResponseDTO>();
            responseDTO.AccessToken = _jwtAccessGenerator.GetAccessToken(new Dictionary<string, string>()
            {
                {JwtRegisteredClaimNames.Sub, externalIdentityProvider.IdentityUserFK.Id.ToString()},
            }).Token;
            return new CommitResult<LoginResponseDTO>
            {
                ResultType = ResultType.Ok,
                Value = responseDTO
            };
        }
    }
}