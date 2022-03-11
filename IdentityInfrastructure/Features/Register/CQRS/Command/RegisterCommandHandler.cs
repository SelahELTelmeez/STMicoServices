using IdentityDomain.Features.Register.CQRS.Command;
using IdentityDomain.Features.Register.DTO.Command;
using IdentityDomain.Models;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using Mapster;
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
        public RegisterCommandHandler(STIdentityDbContext dbContext, JsonLocalizerManager resourceJsonManager, TokenHandlerManager tokenHandlerManager, INotificationService notificationService)
        {
            _dbContext = dbContext;
            _resourceJsonManager = resourceJsonManager;
            _jwtAccessGenerator = tokenHandlerManager;
            _notificationService = notificationService;
        }
        public async Task<CommitResult<RegisterResponseDTO>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // 1.0 Check for the user existance first, with the provided data.
            bool isEmailUsed = !string.IsNullOrWhiteSpace(request.IdentityRegisterRequest.Email);
            IdentityUser? identityUser;
            if (isEmailUsed)
            {
                identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Email.Equals(request.IdentityRegisterRequest.Email), cancellationToken);
            }
            else
            {
                identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.MobileNumber.Equals(request.IdentityRegisterRequest.MobileNumber), cancellationToken);
            }
            if (identityUser == null)
            {
                return new CommitResult<RegisterResponseDTO>
                {
                    ErrorCode = "X0004",
                    ErrorMessage = _resourceJsonManager["X0004"], // Duplicated User data, try to sign in instead.
                    ResultType = ResultType.Invalid, // TODO: Add Result Type: Duplicated
                };
            }
            else
            {
                //2.0 Start Adding the user to the databse.
                // Add Mapping here.
                IdentityUser user = request.IdentityRegisterRequest.Adapt<IdentityUser>();
                _dbContext.Set<IdentityUser>().Add(user);
                await _dbContext.SaveChangesAsync(cancellationToken);

                // 3.0 Return a mapped response.
                await _dbContext.Entry(user).Reference(a => a.AvatarFK).LoadAsync(cancellationToken);
                await _dbContext.Entry(user).Reference(a => a.GradeFK).LoadAsync(cancellationToken);
                await _dbContext.Entry(user).Reference(a => a.IdentityRoleFK).LoadAsync(cancellationToken);
                await _dbContext.Entry(user).Reference(a => a.GovernorateFK).LoadAsync(cancellationToken);
                RegisterResponseDTO responseDTO = user.Adapt<RegisterResponseDTO>();
                AccessToken accessToken = _jwtAccessGenerator.GetAccessToken(new Dictionary<string, string>()
                {
                    {JwtRegisteredClaimNames.Sub, user.Id.ToString()},
                });
                RefreshToken refreshToken = _jwtAccessGenerator.GetRefreshToken();
                _dbContext.Set<RefreshToken>().Add(refreshToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                responseDTO.RefreshToken = refreshToken.Token;
                responseDTO.AccessToken = accessToken.Token;

                // 4.0 SEND Email OR SMS
                if (isEmailUsed)
                {
                    // ADD TO LOG
                    _ = _notificationService.SendEmailAsync(new EmailNotificationModel
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
                    _ = _notificationService.SendSMSAsync(new SMSNotificationModel
                    {
                        MobileNumber = user.MobileNumber,
                        OTPCode = user.Activations.FirstOrDefault()?.Code
                    }, cancellationToken);
                }

                return new CommitResult<RegisterResponseDTO>
                {
                    ResultType = ResultType.Ok,
                    Value = responseDTO
                };
            }
        }
    }
}
