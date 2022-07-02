using IdentityDomain.Features.Integration.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.IdentityModel.Tokens.Jwt;

namespace IdentityInfrastructure.Features.Integration.CQRS.Command;

public class RegisterExternalUserCommandHandler : IRequestHandler<RegisterExternalUserCommand, CommitResult<string>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly TokenHandlerManager _jwtAccessGenerator;
    private readonly IMediator _mediator;
    public RegisterExternalUserCommandHandler(STIdentityDbContext dbContext, TokenHandlerManager jwtAccessGenerator, IMediator mediator)
    {
        _dbContext = dbContext;
        _jwtAccessGenerator = jwtAccessGenerator;
        _mediator = mediator;
    }

    public async Task<CommitResult<string>> Handle(RegisterExternalUserCommand request, CancellationToken cancellationToken)
    {

        //========== Check the existance of the external user before, if yes, return the user token ===============================

        CommitResult<string> externalUser = await _mediator.Send(new VerifyExternalUserCommand(request.ExternalUserRegisterRequest.ExternalUserId, request.ExternalUserRegisterRequest.ProviderName), cancellationToken);

        if (externalUser.IsSuccess)
        {
            return externalUser;
        }

        //=========** Create a new external user **=========

        IdentitySchool? identitySchool = await _dbContext.Set<IdentitySchool>().SingleOrDefaultAsync(a => a.Name == request.ExternalUserRegisterRequest.ProviderName, cancellationToken);

        if (identitySchool == null)
        {
            return new CommitResult<string>
            {
                ResultType = ResultType.Invalid,
                ErrorCode = "XIDN00021",
                ErrorMessage = "The provider isn't registered, please contact with Selaheltelmeez's administrator",
                Value = default
            };
        }

        IdentityUser user = new IdentityUser
        {
            FullName = request.ExternalUserRegisterRequest.FullName,
            Email = request.ExternalUserRegisterRequest.Email?.Trim()?.ToLower(),
            MobileNumber = request.ExternalUserRegisterRequest.MobileNumber,
            GradeId = request.ExternalUserRegisterRequest.GradeId,
            AvatarId = 0,
            IsPremium = true,
            IdentityRoleId = request.ExternalUserRegisterRequest.RoleId,
            IsEmailVerified = request.ExternalUserRegisterRequest.Email != null,
            IsMobileVerified = request.ExternalUserRegisterRequest.MobileNumber != null,
            ReferralCode = UtilityGenerator.GetUniqueDigits(),
            IsExternalUser = true,
            ExternalUserId = request.ExternalUserRegisterRequest.ExternalUserId,
            IdentitySchoolId = identitySchool.Id
        };

        _dbContext.Set<IdentityUser>().Add(user);


        await _dbContext.SaveChangesAsync(cancellationToken);

        // Generate Both Access and Refresh Tokens
        AccessToken accessToken = _jwtAccessGenerator.GetAccessToken(new Dictionary<string, string>()
        {
            {JwtRegisteredClaimNames.Sub, user.Id.ToString()},
        });

        return new CommitResult<string>
        {
            Value = accessToken.Token
        };
    }
}
