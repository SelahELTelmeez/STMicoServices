using IdentityDomain.Features.Login.CQRS.Command;
using IdentityDomain.Features.Login.DTO.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using JWTGenerator.TokenHandler;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.Login.CQRS.Command;
public class IdentityLoginCommandHandler : IRequestHandler<IdentityLoginCommand, CommitResult<IdentityLoginResponseDTO>>
{
    private readonly AuthenticationDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly TokenHandlerManager _jwtAccessGenerator;
    public IdentityLoginCommandHandler(AuthenticationDbContext dbContext, JsonLocalizerManager resourceJsonManager, TokenHandlerManager tokenHandlerManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
        _jwtAccessGenerator = tokenHandlerManager;
    }
    public async Task<CommitResult<IdentityLoginResponseDTO>> Handle(IdentityLoginCommand request, CancellationToken cancellationToken)
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
                return new CommitResult<IdentityLoginResponseDTO>
                {
                    ErrorCode = "X0002",
                    ErrorMessage = _resourceJsonManager["X0002"],
                    ResultType = ResultType.NotFound,
                };
            }
            else
            {
                return new CommitResult<IdentityLoginResponseDTO>
                {
                    ResultType = ResultType.Ok,
                    Value = identityUser.Adapt<IdentityLoginResponseDTO>()
                };
            }
        }
        // check by mobile number
        if (!string.IsNullOrEmpty(request.IdentityLoginRequest.MobileNumber))
        {
            IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.MobileNumber == request.IdentityLoginRequest.MobileNumber && a.PasswordHash == request.IdentityLoginRequest.PasswordHash);
            if (identityUser == null)
            {
                return new CommitResult<IdentityLoginResponseDTO>
                {
                    ErrorCode = "X0002",
                    ErrorMessage = _resourceJsonManager["X0002"],
                    ResultType = ResultType.NotFound,
                };
            }
            else
            {
                return new CommitResult<IdentityLoginResponseDTO>
                {
                    ResultType = ResultType.Ok,
                    Value = identityUser.Adapt<IdentityLoginResponseDTO>()
                };
            }
        }
        return new CommitResult<IdentityLoginResponseDTO>
        {
            ErrorCode = "X0003",
            ErrorMessage = _resourceJsonManager["X0003"],
            ResultType = ResultType.Invalid,
        };
    }
    private async Task<CommitResult<IdentityLoginResponseDTO>> GetExternalProviderAsync(string providerId, string providerName, CancellationToken cancellationToken)
    {
        ExternalIdentityProvider? externalIdentityProvider = await _dbContext.Set<ExternalIdentityProvider>().SingleOrDefaultAsync(a => a.Name == providerName && a.ProviderId == providerId, cancellationToken);
        if (externalIdentityProvider == null)
        {
            return new CommitResult<IdentityLoginResponseDTO>
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
            IdentityLoginResponseDTO responseDTO = externalIdentityProvider.IdentityUserFK.Adapt<IdentityLoginResponseDTO>();
            responseDTO.AccessToken = _jwtAccessGenerator.GetAccessToken(new Dictionary<string, string>()
            {
                {"IdentityId", externalIdentityProvider.IdentityUserFK.Id.ToString()},
            }).Token;
            return new CommitResult<IdentityLoginResponseDTO>
            {
                ResultType = ResultType.Ok,
                Value = responseDTO
            };
        }
    }
}