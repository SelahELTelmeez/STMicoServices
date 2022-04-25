using IdentityDomain.Features.IdentityUserTransaction.CQRS.Command;
using IdentityDomain.Features.IdentityUserTransaction.DTO;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.IdentityModel.Tokens.Jwt;

namespace IdentityInfrastructure.Features.IdentityUserTransaction.CQRS.Command
{
    public class AddNewChildCommandHandler : IRequestHandler<AddNewChildCommand, CommitResult<AddNewChildResponse>>
    {
        private readonly STIdentityDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;
        private readonly TokenHandlerManager _jwtAccessGenerator;
        private readonly Guid? _userId;

        public AddNewChildCommandHandler(STIdentityDbContext dbContext,
                                      IWebHostEnvironment configuration,
                                      IHttpContextAccessor httpContextAccessor,
                                      TokenHandlerManager tokenHandlerManager)
        {
            _dbContext = dbContext;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
            _jwtAccessGenerator = tokenHandlerManager;
            _userId = httpContextAccessor.GetIdentityUserId();
        }
        public async Task<CommitResult<AddNewChildResponse>> Handle(AddNewChildCommand request, CancellationToken cancellationToken)
        {

            if (request.AddNewChildRequest.ChildId == null)
            {
                // 1.0 Check for the child already added to this parent
                // check for duplicated data.
                IdentityRelation? identityUser = await _dbContext.Set<IdentityRelation>().SingleOrDefaultAsync(a => a.RelationType == (RelationType)1
                                                                                                                 && a.PrimaryId.Equals(_userId)
                                                                                                                 && a.SecondaryFK.FullName.Equals(request.AddNewChildRequest.FullName), cancellationToken);
                if (identityUser != null)
                {
                    // in case of the duplicated data is not validated, then delete the old ones.
                    return new CommitResult<AddNewChildResponse>
                    {
                        ErrorCode = "X0010",
                        ErrorMessage = _resourceJsonManager["X0010"], // Duplicated User data
                        ResultType = ResultType.Invalid, // TODO: Add Result Type: Duplicated
                    };
                }

                //2.0 Start Adding the user to the databse.
                IdentityUser user = new IdentityUser
                {
                    FullName = request.AddNewChildRequest.FullName,
                    DateOfBirth = request.AddNewChildRequest.DateOfBirth,
                    Gender = request.AddNewChildRequest.Gender,
                    //ExternalIdentityProviders = request.RegisterRequest.GetExternalProviders(),
                    ReferralCode = UtilityGenerator.GetUniqueDigits(),
                    GradeId = request.AddNewChildRequest.GradeId,
                    AvatarId = 0,
                    IsPremium = false,
                    IdentityRoleId = 1, //student
                };

                _dbContext.Set<IdentityUser>().Add(user);
                await _dbContext.SaveChangesAsync(cancellationToken);

                // 3.0 load related entities and Map their values.
                AddNewChildResponse responseDTO = await LoadRelatedEntitiesAsync(user, cancellationToken);
                if (responseDTO == null)
                {
                    return new CommitResult<AddNewChildResponse>
                    {
                        ErrorCode = "X0011",
                        ErrorMessage = _resourceJsonManager["X0011"], // Invalid Operation
                        ResultType = ResultType.Invalid,
                    };
                }

                // 4.0 connect paren to his child.
                IdentityRelation IdentityRelation = new IdentityRelation
                {
                    RelationType = (RelationType)1,  //ParentToChild
                    PrimaryId = _userId,
                    SecondaryId = responseDTO.Id,
                };
                _dbContext.Set<IdentityRelation>().Add(IdentityRelation);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return new CommitResult<AddNewChildResponse>
                {
                    ResultType = ResultType.Ok,
                    Value = responseDTO
                };
            }

            else
            {
                IdentityRelation? identityUser = await _dbContext.Set<IdentityRelation>().SingleOrDefaultAsync(a => a.RelationType == (RelationType)1 && a.PrimaryId.Equals(_userId) &&
                                                                                                               a.SecondaryId.Equals(request.AddNewChildRequest.ChildId), cancellationToken);
                if (identityUser != null)
                {
                    // in case of the duplicated data is not validated, then delete the old ones.
                    return new CommitResult<AddNewChildResponse>
                    {
                        ErrorCode = "X0010",
                        ErrorMessage = _resourceJsonManager["X0010"], // Duplicated User data
                        ResultType = ResultType.Invalid, // TODO: Add Result Type: Duplicated
                    };
                }

                // 4.0 connect paren to his child.
                IdentityRelation IdentityRelation = new IdentityRelation
                {
                    RelationType = (RelationType)1,
                    PrimaryId = _userId,
                    SecondaryId = request.AddNewChildRequest.ChildId,
                };
                _dbContext.Set<IdentityRelation>().Add(IdentityRelation);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return new CommitResult<AddNewChildResponse>
                {
                    ResultType = ResultType.Ok
                };
            }
        }

        private async Task<AddNewChildResponse> LoadRelatedEntitiesAsync(IdentityUser identityUser, CancellationToken cancellationToken)
        {
            // Loading Related Entities
            await _dbContext.Entry(identityUser).Reference(a => a.GradeFK).LoadAsync(cancellationToken);
            await _dbContext.Entry(identityUser).Reference(a => a.IdentityRoleFK).LoadAsync(cancellationToken);

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

            // Mapping To return the result to the User.
            AddNewChildResponse responseDTO = new AddNewChildResponse
            {
                Id = identityUser.Id,
                FullName = identityUser.FullName,
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken.Token,
                Grade = identityUser?.GradeFK?.Name,
                DateOfBirth = identityUser?.DateOfBirth,
                IsPremium = false,
                ReferralCode = identityUser.ReferralCode,
                Role = identityUser.IdentityRoleFK.Name
            };
            await _dbContext.SaveChangesAsync(cancellationToken);

            return responseDTO;
        }
    }
}