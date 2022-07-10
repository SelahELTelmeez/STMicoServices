using Flaminco.CommitResult;
using IdentityDomain.Features.Integration.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace IdentityInfrastructure.Features.Integration.CQRS.Command;

public class VerifyExternalUserCommandHandler : IRequestHandler<VerifyExternalUserCommand, ICommitResult<string>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly TokenHandlerManager _jwtAccessGenerator;

    public VerifyExternalUserCommandHandler(STIdentityDbContext dbContext, TokenHandlerManager jwtAccessGenerator)
    {
        _dbContext = dbContext;
        _jwtAccessGenerator = jwtAccessGenerator;
    }
    public async Task<ICommitResult<string>> Handle(VerifyExternalUserCommand request, CancellationToken cancellationToken)
    {

        IdentitySchool? identitySchool = await _dbContext.Set<IdentitySchool>().FirstOrDefaultAsync(a => a.ProviderSecretKey == request.ProviderSecretKey, cancellationToken);

        if (identitySchool == null)
        {
            return ResultType.NotFound.GetValueCommitResult(string.Empty, "XIDN00020", "Invalid Provider, please contact with Selaheltelmeez's administrator");
        }

        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>()
                                                     .Where(a => a.IsExternalUser == true)
                                                     .Where(a => a.ExternalUserId == request.ExternalUserId)
                                                     .Where(a => a.IdentitySchoolId == identitySchool.Id)
                                                     .FirstOrDefaultAsync(cancellationToken);

        if (identityUser == null)
        {
            return ResultType.NotFound.GetValueCommitResult(string.Empty, "XIDN00021", "The current user isn't registered");
        }

        // Generate Both Access and Refresh Tokens
        AccessToken accessToken = _jwtAccessGenerator.GetAccessToken(new Dictionary<string, string>()
        {
            {JwtRegisteredClaimNames.Sub, identityUser!.Id.ToString()},
        });

        return ResultType.Ok.GetValueCommitResult(accessToken.Token);
    }
}
