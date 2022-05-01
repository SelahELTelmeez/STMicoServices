using IdentityDomain.Features.Refresh.CQRS.Command;
using IdentityDomain.Features.Refresh.DTO.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.IdentityModel.Tokens.Jwt;

namespace IdentityInfrastructure.Features.Refresh.CQRS.Command;
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, CommitResult<RefreshTokenResponse>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly STIdentityDbContext _dbContext;
    private readonly TokenHandlerManager _jwtAccessGenerator;
    private readonly JsonLocalizerManager _resourceJsonManager;
    public RefreshTokenCommandHandler(STIdentityDbContext dbContext,
                                        IWebHostEnvironment configuration,
                                        IHttpContextAccessor httpContextAccessor,
                                        TokenHandlerManager tokenHandlerManager)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _jwtAccessGenerator = tokenHandlerManager;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }
    public async Task<CommitResult<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        IdentityRefreshToken? refreshToken = await _dbContext.Set<IdentityRefreshToken>().SingleOrDefaultAsync(a => a.Token.Equals(request.RefreshToken), cancellationToken);
        if (refreshToken == null)
        {
            return new CommitResult<RefreshTokenResponse>
            {
                ErrorCode = "X0008",
                ErrorMessage = _resourceJsonManager["X0008"],
                ResultType = ResultType.Unauthorized
            };
        }
        else
        {
            if (refreshToken.IsActive)
            {
                // Revoke the old refresh token
                refreshToken.RevokedOn = DateTime.UtcNow;

                // generate a new access and refresh tokens
                AccessToken newAccessToken = _jwtAccessGenerator.GetAccessToken(new Dictionary<string, string>()
                {
                    {JwtRegisteredClaimNames.Sub, _httpContextAccessor.GetIdentityUserId().ToString()},
                });
                RefreshToken newRefreshToken = _jwtAccessGenerator.GetRefreshToken();
                _dbContext.Set<IdentityRefreshToken>().Add(new IdentityRefreshToken
                {
                    Token = newRefreshToken.Token,
                    CreatedOn = newRefreshToken.CreatedOn,
                    ExpiresOn = newRefreshToken.ExpiresOn,
                    RevokedOn = newRefreshToken.RevokedOn,
                    IdentityUserId = refreshToken.IdentityUserId
                });
                await _dbContext.SaveChangesAsync();

                return new CommitResult<RefreshTokenResponse>
                {
                    ResultType = ResultType.Ok,
                    Value = new RefreshTokenResponse
                    {
                        RefreshToken = newRefreshToken.Token,
                        AccessToken = newAccessToken.Token
                    }
                };
            }
            else
            {
                return new CommitResult<RefreshTokenResponse>
                {
                    ErrorCode = "X0009",
                    ErrorMessage = _resourceJsonManager["X0009"],
                    ResultType = ResultType.Unauthorized
                };
            }
        }
    }
}
