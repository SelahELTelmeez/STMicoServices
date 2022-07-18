using Flaminco.CommitResult;
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
using System.IdentityModel.Tokens.Jwt;

namespace IdentityInfrastructure.Features.Refresh.CQRS.Command;
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ICommitResult<RefreshTokenResponse>>
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
    public async Task<ICommitResult<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        IdentityRefreshToken? refreshToken = await _dbContext.Set<IdentityRefreshToken>().FirstOrDefaultAsync(a => a.Token.Equals(request.RefreshToken), cancellationToken);
        if (refreshToken == null)
        {
            return ResultType.Unauthorized.GetValueCommitResult((RefreshTokenResponse)null, "XIDN0008", _resourceJsonManager["XIDN0008"]);
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

                return ResultType.Ok.GetValueCommitResult(new RefreshTokenResponse
                {
                    RefreshToken = newRefreshToken.Token,
                    AccessToken = newAccessToken.Token
                });
            }
            else
            {
                return ResultType.Unauthorized.GetValueCommitResult((RefreshTokenResponse)null, "XIDN0009", _resourceJsonManager["XIDN0009"]);
            }
        }
    }
}
