﻿using IdentityDomain.Features.Register.CQRS.Command;
using IdentityDomain.Features.Register.DTO.Command;
using IdentityDomain.Models;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ResultHandler;
using System.IdentityModel.Tokens.Jwt;

namespace IdentityInfrastructure.Features.Register.CQRS.Command
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, CommitResult<RegisterResponseDTO>>
    {
        private readonly AuthenticationDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;
        private readonly TokenHandlerManager _jwtAccessGenerator;
        private readonly INotificationEmailService _notificationEmailService;
        private readonly IConfiguration _configuration;
        public RegisterCommandHandler(AuthenticationDbContext dbContext, JsonLocalizerManager resourceJsonManager, TokenHandlerManager tokenHandlerManager, INotificationEmailService notificationEmailService, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _resourceJsonManager = resourceJsonManager;
            _jwtAccessGenerator = tokenHandlerManager;
            _notificationEmailService = notificationEmailService;
            _configuration = configuration;
        }
        public async Task<CommitResult<RegisterResponseDTO>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // 1.0 Check for the user existance first, with the provided data.
            bool isEmailUsed = !string.IsNullOrWhiteSpace(request.IdentityRegisterRequest.Email);
            IdentityUser? identityUser;
            if (isEmailUsed)
            {
                identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Email.Equals(request.IdentityRegisterRequest.Email));
            }
            else
            {
                identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.MobileNumber.Equals(request.IdentityRegisterRequest.MobileNumber));
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
                await _dbContext.SaveChangesAsync();

                // 3.0 Send Email OR SMS
                ///TODO: Add OTP Generator here.
                if (isEmailUsed)
                {
                    _ = _notificationEmailService.SendAsync(new EmailNotificationModel
                    {
                        MailFrom = "noreply@selaheltelmeez.com",
                        MailTo = user.Email,
                        MailSubject = "سلاح التلميذ - رمز التفعيل",
                        IsBodyHtml = true,
                        DisplayName = "سلاح التلميذ",
                        MailToName = user.FullName,
                        MailBody = UtilityGenerator.GetOTP(4).ToString()
                    }, cancellationToken);
                }
                else
                {

                }

                // 4.0 Return a mapped response.
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
                await _dbContext.SaveChangesAsync();
                responseDTO.RefreshToken = refreshToken.Token;
                responseDTO.AccessToken = accessToken.Token;
                return new CommitResult<RegisterResponseDTO>
                {
                    ResultType = ResultType.Ok,
                    Value = responseDTO
                };
            }
        }
    }
}