using IdentityDomain.Features.Register.CQRS.Command;
using IdentityDomain.Features.Register.DTO.Command;
using IdentityDomain.Models;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Mapping;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.IdentityModel.Tokens.Jwt;

namespace IdentityInfrastructure.Features.Register.CQRS.Command
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, CommitResult<RegisterResponseDTO>>
    {
        private readonly STIdentityDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;
        private readonly TokenHandlerManager _jwtAccessGenerator;
        private readonly INotificationService _notificationService;
        public RegisterCommandHandler(STIdentityDbContext dbContext,
                                      IWebHostEnvironment configuration,
                                      IHttpContextAccessor httpContextAccessor,
                                      TokenHandlerManager tokenHandlerManager,
                                      INotificationService notificationService)
        {
            _dbContext = dbContext;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
            _jwtAccessGenerator = tokenHandlerManager;
            _notificationService = notificationService;
        }
        public async Task<CommitResult<RegisterResponseDTO>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // 1.0 Check for the user existance first, with the provided data.
            bool isEmailUsed = !string.IsNullOrWhiteSpace(request.RegisterRequest.Email);
            IdentityUser? identityUser;
            if (isEmailUsed)
            {
                identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Email.Equals(request.RegisterRequest.Email), cancellationToken);

            }
            else
            {
                identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.MobileNumber.Equals(request.RegisterRequest.MobileNumber), cancellationToken);
            }
            if (identityUser != null)
            {
                return new CommitResult<RegisterResponseDTO>
                {
                    ErrorCode = "X0001",
                    ErrorMessage = _resourceJsonManager["X0001"], // Duplicated User data, try to sign in instead.
                    ResultType = ResultType.Invalid, // TODO: Add Result Type: Duplicated
                };
            }
            else
            {
                //2.0 Start Adding the user to the databse.
                IdentityUser user = new IdentityUser
                {
                    FullName = request.RegisterRequest.FullName,
                    Email = request.RegisterRequest.Email,
                    MobileNumber = request.RegisterRequest.MobileNumber,
                    PasswordHash = request.RegisterRequest.PasswordHash,
                    ExternalIdentityProviders = request.RegisterRequest.GetExternalProviders(),
                    Activations = request.RegisterRequest.GenerateOTP(),
                    ReferralCode = UtilityGenerator.GetUniqueDigits(),
                    GradeId = request.RegisterRequest.GradeId,
                    AvatarId = 0,
                    IsPremium = false,
                    IdentityRoleId = request.RegisterRequest.IdentityRoleId
                };



                _dbContext.Set<IdentityUser>().Add(user);
                await _dbContext.SaveChangesAsync(cancellationToken);

                // 3.0 load related entities and Map their values.
                RegisterResponseDTO responseDTO = await LoadRelatedEntitiesAsync(user, cancellationToken);

                if (responseDTO == null)
                {
                    return new CommitResult<RegisterResponseDTO>
                    {
                        ErrorCode = "X0000",
                        ErrorMessage = _resourceJsonManager["X0000"], // Invalid Operation
                        ResultType = ResultType.Invalid,
                    };
                }

                // 4.0 SEND Email OR SMS
                if (isEmailUsed)
                {
                    // ADD TO LOG
                    await _notificationService.SendEmailAsync(new EmailNotificationModel
                    {
                        MailFrom = "noreply@selaheltelmeez.com",
                        MailTo = user.Email,
                        MailSubject = "سلاح التلميذ - رمز التفعيل",
                        IsBodyHtml = true,
                        DisplayName = "سلاح التلميذ",
                        MailToName = user.FullName,
                        MailBody = user.Activations.FirstOrDefault()?.Code
                    }, cancellationToken);
                }
                else
                {
                    // ADD TO LOG
                    await _notificationService.SendSMSAsync(new SMSNotificationModel
                    {
                        Mobile = user.MobileNumber,
                        Code = user.Activations.FirstOrDefault()?.Code
                    }, cancellationToken);
                }
                return new CommitResult<RegisterResponseDTO>
                {
                    ResultType = ResultType.Ok,
                    Value = responseDTO
                };
            }
        }

        private async Task<RegisterResponseDTO> LoadRelatedEntitiesAsync(IdentityUser identityUser, CancellationToken cancellationToken)
        {
            // Loading Related Entities
            await _dbContext.Entry(identityUser).Reference(a => a.AvatarFK).LoadAsync(cancellationToken);
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
            RegisterResponseDTO responseDTO = new RegisterResponseDTO
            {
                FullName = identityUser.FullName,
                Email = identityUser.Email,
                MobileNumber = identityUser.MobileNumber,
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken.Token,
                AvatarUrl = $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{Enum.GetName(typeof(AvatarType), AvatarType.Default)}/{identityUser.AvatarFK.ImageUrl}",
                Grade = identityUser?.GradeFK?.Name,
                IsPremium = false,
                IsVerified = false,
                ReferralCode = identityUser.ReferralCode,
                Role = identityUser.IdentityRoleFK.Name
            };


            await _dbContext.SaveChangesAsync(cancellationToken);

            return responseDTO;
        }
    }
}
