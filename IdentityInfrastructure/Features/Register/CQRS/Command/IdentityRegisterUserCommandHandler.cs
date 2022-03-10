﻿using IdentityDomain.Features.Register.CQRS.Command;
using IdentityDomain.Features.Register.DTO.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using JWTGenerator.TokenHandler;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.Register.CQRS.Command
{
    public class IdentityRegisterUserCommandHandler : IRequestHandler<IdentityRegisterUserCommand, CommitResult<IdentityRegisterResponseDTO>>
    {
        private readonly AuthenticationDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;
        private readonly TokenHandlerManager _jwtAccessGenerator;
        public IdentityRegisterUserCommandHandler(AuthenticationDbContext dbContext, JsonLocalizerManager resourceJsonManager, TokenHandlerManager tokenHandlerManager)
        {
            _dbContext = dbContext;
            _resourceJsonManager = resourceJsonManager;
            _jwtAccessGenerator = tokenHandlerManager;
        }
        public async Task<CommitResult<IdentityRegisterResponseDTO>> Handle(IdentityRegisterUserCommand request, CancellationToken cancellationToken)
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
                return new CommitResult<IdentityRegisterResponseDTO>
                {
                    ErrorCode = "X0004",
                    ErrorMessage = _resourceJsonManager["X0004"], // Duplicated User data, try to sign in instead.
                    ResultType = ResultType.Invalid,
                };
            }
            else
            {
                //2.0 Start Adding the user to the databse.
                // Add Mapping here.
                _dbContext.Set<IdentityUser>().Add(new IdentityUser
                {
                    MobileNumber = request.IdentityRegisterRequest.MobileNumber,
                    Email = request.IdentityRegisterRequest.Email,
                    Gender = identityUser.Gender,

                });

                // 3.0 Send Email OR SMS


                // 4.0 Return a mapped response.
                return new CommitResult<IdentityRegisterResponseDTO>
                {
                    ResultType = ResultType.Ok,
                    Value = new IdentityRegisterResponseDTO
                    {

                        AccessToken = _jwtAccessGenerator.GetAccessToken(new Dictionary<string, string>()).Token
                    }
                };
            }
        }
    }
}