using IdentityDomain.Features.Refresh.CQRS.Command;
using IdentityDomain.Features.Refresh.DTO.Command;
using IdentityEntities.Entities;
using IdentityInfrastructure.Utilities;
using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.IdentityModel.Tokens.Jwt;

namespace IdentityInfrastructure.Features.Refresh.CQRS.Command;
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, CommitResult<RefreshTokenResponseDTO>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly STIdentityDbContext _dbContext;
    private readonly TokenHandlerManager _jwtAccessGenerator;
    public RefreshTokenCommandHandler(STIdentityDbContext dbContext, IHttpContextAccessor httpContextAccessor, TokenHandlerManager tokenHandlerManager)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _jwtAccessGenerator = tokenHandlerManager;
    }
    public async Task<CommitResult<RefreshTokenResponseDTO>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        RefreshToken? refreshToken = await _dbContext.Set<RefreshToken>().SingleOrDefaultAsync(a => a.Token.Equals(request.RefreshToken));
        if (refreshToken == null)
        {
            return new CommitResult<RefreshTokenResponseDTO>
            {
                ErrorCode = "X0005",
                ErrorMessage = "X0005",
                ResultType = ResultType.Unauthorized
            };
        }
        else
        {
            if (refreshToken.IsActive)
            {
                AccessToken newAccessToken = _jwtAccessGenerator.GetAccessToken(new Dictionary<string, string>()
                {
                    {JwtRegisteredClaimNames.Sub, HttpIdentityUser.GetIdentityUserId(_httpContextAccessor)},
                });
                RefreshToken newRefreshToken = _jwtAccessGenerator.GetRefreshToken();
                refreshToken.RevokedOn = DateTime.UtcNow;
                _dbContext.Set<RefreshToken>().Add(newRefreshToken);
                await _dbContext.SaveChangesAsync();
                return new CommitResult<RefreshTokenResponseDTO>
                {
                    ResultType = ResultType.Ok,
                    Value = new RefreshTokenResponseDTO
                    {
                        RefreshToken = newRefreshToken.Token,
                        AccessToken = newAccessToken.Token
                    }
                };
            }
            else
            {
                return new CommitResult<RefreshTokenResponseDTO>
                {
                    ErrorCode = "X007", // Token is not active
                    ErrorMessage = "",
                    ResultType = ResultType.Unauthorized
                };
            }
        }
    }
}
