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

            // check for duplicated data.
            IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => isEmailUsed ? a.Email.Equals(request.RegisterRequest.Email) :
                                                                                                                      a.MobileNumber.Equals(request.RegisterRequest.MobileNumber), cancellationToken);
            if (identityUser != null)
            {
                // in case of the duplicated data is not validated, then delete the old ones.
                if (!(identityUser.IsEmailVerified.GetValueOrDefault() && identityUser.IsMobileVerified.GetValueOrDefault()))
                {
                    _dbContext.Set<IdentityUser>().Remove(identityUser);
                }
                else if (!identityUser.IsMobileVerified.GetValueOrDefault() && !isEmailUsed)
                {
                    identityUser.MobileNumber = null;
                    _dbContext.Set<IdentityUser>().Update(identityUser);
                }
                else if (!identityUser.IsEmailVerified.GetValueOrDefault() && isEmailUsed)
                {
                    identityUser.Email = null;
                    _dbContext.Set<IdentityUser>().Update(identityUser);
                }
                else
                {
                    return new CommitResult<RegisterResponseDTO>
                    {
                        ErrorCode = "X0010",
                        ErrorMessage = _resourceJsonManager["X0010"], // Duplicated User data, try to sign in instead.
                        ResultType = ResultType.Invalid, // TODO: Add Result Type: Duplicated
                    };
                }
            }
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
                IdentityRoleId = request.RegisterRequest.IdentityRoleId,
                IsEmailVerified = isEmailUsed ? false : null,
                IsMobileVerified = isEmailUsed ? null : false
            };
            _dbContext.Set<IdentityUser>().Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // 3.0 load related entities and Map their values.
            RegisterResponseDTO responseDTO = await LoadRelatedEntitiesAsync(user, cancellationToken);
            if (responseDTO == null)
            {
                return new CommitResult<RegisterResponseDTO>
                {
                    ErrorCode = "X0011",
                    ErrorMessage = _resourceJsonManager["X0011"], // Invalid Operation
                    ResultType = ResultType.Invalid,
                };
            }

            // 4.0 SEND Email OR SMS
            bool sendResult = isEmailUsed ? await _notificationService.SendEmailAsync(new EmailNotificationModel
            {
                MailFrom = "noreply@selaheltelmeez.com",
                MailTo = user.Email,
                MailSubject = "سلاح التلميذ - رمز التفعيل",
                IsBodyHtml = true,
                DisplayName = "سلاح التلميذ",
                MailToName = user.FullName,
                MailBody = user.Activations.FirstOrDefault()?.Code
            }, cancellationToken) : await _notificationService.SendSMSAsync(new SMSNotificationModel
            {
                Mobile = user.MobileNumber,
                Code = user.Activations.FirstOrDefault()?.Code
            }, cancellationToken);

            return new CommitResult<RegisterResponseDTO>
            {
                ResultType = sendResult ? ResultType.Ok : ResultType.PartialOk,
                Value = responseDTO
            };
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
                Id = identityUser.Id.ToString(),
                FullName = identityUser.FullName,
                Email = identityUser.Email,
                MobileNumber = identityUser.MobileNumber,
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken.Token,
                AvatarUrl = $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{Enum.GetName(typeof(AvatarType), AvatarType.Default)}/{identityUser.AvatarFK.ImageUrl}",
                Grade = identityUser?.GradeFK?.Name,
                IsPremium = false,
                IsEmailVerified = false,
                IsMobileVerified = false,
                ReferralCode = identityUser.ReferralCode,
                Role = identityUser.IdentityRoleFK.Name
            };

            await _dbContext.SaveChangesAsync(cancellationToken);

            return responseDTO;
        }
    }
}
