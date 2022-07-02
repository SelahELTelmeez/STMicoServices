using IdentityDomain.Features.Integration.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.IdentityModel.Tokens.Jwt;

namespace IdentityInfrastructure.Features.Integration.CQRS.Command;

public class VerifyExternalUserCommandHandler : IRequestHandler<VerifyExternalUserCommand, CommitResult<string>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly TokenHandlerManager _jwtAccessGenerator;

    public VerifyExternalUserCommandHandler(STIdentityDbContext dbContext, TokenHandlerManager jwtAccessGenerator)
    {
        _dbContext = dbContext;
        _jwtAccessGenerator = jwtAccessGenerator;
    }
    public async Task<CommitResult<string>> Handle(VerifyExternalUserCommand request, CancellationToken cancellationToken)
    {

        IdentitySchool? identitySchool = await _dbContext.Set<IdentitySchool>().FirstOrDefaultAsync(a => a.ProviderSecretKey == request.ProviderSecretKey, cancellationToken);

        if (identitySchool == null)
        {
            return new CommitResult<string>()
            {
                ResultType = ResultType.Invalid,
                ErrorCode = "XIDN00020",
                ErrorMessage = "Invalid Provider, please contact with Selaheltelmeez's administrator"
            };
        }

        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>()
                                                     .Where(a => a.IsExternalUser == true)
                                                     .Where(a => a.ExternalUserId == request.ExternalUserId)
                                                     .Where(a => a.IdentitySchoolId == identitySchool.Id)
                                                     .FirstOrDefaultAsync(cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult<string>
            {
                ResultType = ResultType.NotFound,
                ErrorCode = "XIDN00021",
                ErrorMessage = "The current user isn't registered",
                Value = default
            };
        }

        // Generate Both Access and Refresh Tokens
        AccessToken accessToken = _jwtAccessGenerator.GetAccessToken(new Dictionary<string, string>()
        {
            {JwtRegisteredClaimNames.Sub, identityUser!.Id.ToString()},
        });

        return new CommitResult<string>
        {
            Value = accessToken.Token
        };
    }
}
