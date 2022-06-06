using IdentityDomain.Features.Register.CQRS.Command;
using IdentityDomain.Features.Register.DTO.Command;
using IdentityDomain.Models;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Mapping;
using JsonLocalizer;
using JWTGenerator.TokenHandler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.Register.CQRS.Command
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, CommitResult>
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
        public async Task<CommitResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // 1.0 Check for the user existance first, with the provided data.
            bool isEmailUsed = !string.IsNullOrWhiteSpace(request.RegisterRequest.Email);

            // check for duplicated data.
            IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => isEmailUsed ? a.Email.Equals(request.RegisterRequest.Email) :
                                                                                                                      a.MobileNumber.Equals(request.RegisterRequest.MobileNumber), cancellationToken);
            if (identityUser != null)
            {
                // in case of the duplicated data is not validated, then delete the old ones.
                if (identityUser.IsEmailVerified.GetValueOrDefault() == false && identityUser.IsMobileVerified.GetValueOrDefault() == false)
                {
                    _dbContext.Set<IdentityUser>().Remove(identityUser);
                }
                else if (identityUser.IsMobileVerified.GetValueOrDefault() == false && !isEmailUsed)
                {
                    identityUser.MobileNumber = null;
                    _dbContext.Set<IdentityUser>().Update(identityUser);
                }
                else if (identityUser.IsEmailVerified.GetValueOrDefault() == false && isEmailUsed)
                {
                    identityUser.Email = null;
                    _dbContext.Set<IdentityUser>().Update(identityUser);
                }
                else
                {
                    return new CommitResult<RegisterResponse>
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
                Email = request.RegisterRequest.Email?.Trim()?.ToLower(),
                MobileNumber = request.RegisterRequest.MobileNumber,
                PasswordHash = request.RegisterRequest.PasswordHash.Encrypt(true),
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

            return new CommitResult
            {
                ResultType = ResultType.Ok,
            };
        }
    }
}
