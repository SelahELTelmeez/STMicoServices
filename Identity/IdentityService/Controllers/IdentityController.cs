using IdentityDomain.Features.ActivateAccount.CQRS.Command;
using IdentityDomain.Features.ChangeEmailOrMobile.CQRS.Command;
using IdentityDomain.Features.ChangeEmailOrMobile.DTO.Command;
using IdentityDomain.Features.ChangePassword.CQRS.Command;
using IdentityDomain.Features.ChangePassword.DTO.Command;
using IdentityDomain.Features.ConfirmChangeEmailOrMobile.CQRS.Command;
using IdentityDomain.Features.ConfirmForgetPassword.CQRS.Command;
using IdentityDomain.Features.EmailVerification.CQRS.Command;
using IdentityDomain.Features.ExternalIdentityProvider.CQRS.Add.Command;
using IdentityDomain.Features.ExternalIdentityProvider.CQRS.Command;
using IdentityDomain.Features.ExternalIdentityProvider.DTO.Add.Command;
using IdentityDomain.Features.ExternalIdentityProvider.DTO.Remove.Command;
using IdentityDomain.Features.ForgetPassword.CQRS.Command;
using IdentityDomain.Features.ForgetPassword.DTO.Command;
using IdentityDomain.Features.GetUser.CQRS.Query;
using IdentityDomain.Features.IdentityGrade.CQRS.Query;
using IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;
using IdentityDomain.Features.Login.CQRS.Command;
using IdentityDomain.Features.Login.DTO.Command;
using IdentityDomain.Features.MobileVerification.CQRS.Command;
using IdentityDomain.Features.Refresh.CQRS.Command;
using IdentityDomain.Features.Refresh.DTO.Command;
using IdentityDomain.Features.Register.CQRS.Command;
using IdentityDomain.Features.Register.DTO.Command;
using IdentityDomain.Features.ResendEmailVerification.CQRS.Command;
using IdentityDomain.Features.ResendMobileVerification.CQRS.Command;
using IdentityDomain.Features.ResetPassword.CQRS.Command;
using IdentityDomain.Features.ResetPassword.DTO;
using IdentityDomain.Features.Shared.DTO;
using IdentityDomain.Features.UpdateNotificationToken.CQRS.Command;
using IdentityDomain.Features.UpdateProfile.CQRS.Command;
using IdentityDomain.Features.UpdateProfile.DTO.Command;
using IdentityDomain.Features.ValidateToken.CQRS.Command;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ResultHandler;
using SharedModule.DTO;

namespace IdentityService.Controllers;

[ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class IdentityController : ControllerBase
{
    private readonly IMediator _mediator;

    public IdentityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("[action]"), AllowAnonymous, Produces(typeof(CommitResult<LoginResponse>))]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, CancellationToken token)
        => Ok(await _mediator.Send(new LoginCommand(loginRequest), token));

    [HttpGet("[action]"), Produces(typeof(CommitResult<bool>))]
    public async Task<IActionResult> ValidateToken(CancellationToken token)
      => Ok(await _mediator.Send(new ValidateTokenCommand(), token));


    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> ActivateAccount(CancellationToken token)
        => Ok(await _mediator.Send(new ActivateAccountCommand(), token));

    [HttpGet("[action]"), Produces(typeof(CommitResult<LoginResponse>)), AllowAnonymous]
    public async Task<IActionResult> GetUser([FromQuery(Name = "UserId")] string? UserId, CancellationToken token)
       => Ok(await _mediator.Send(new GetUserQuery(UserId), token));

    [HttpGet("[action]"), Produces(typeof(CommitResult<LoginResponse>)), EnableCors(PolicyName = "Allow_Token_Validation_Policy")]
    public async Task<IActionResult> UpdateToken([FromQuery(Name = "NotificationToken")] string NotificationToken, CancellationToken token)
              => Ok(await _mediator.Send(new UpdateNotificationTokenCommand(NotificationToken), token));


    [HttpPost("[action]"), AllowAnonymous, Produces(typeof(CommitResult))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest, CancellationToken token)
         => Ok(await _mediator.Send(new RegisterCommand(registerRequest), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult<UpdateProfileResponse>)), AllowAnonymous]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest updateProfileRequest, CancellationToken token)
     => Ok(await _mediator.Send(new UpdateProfileCommand(updateProfileRequest), token));

    [HttpPost("[action]"), AllowAnonymous, Produces(typeof(CommitResult<RefreshTokenResponse>))]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken, CancellationToken token)
         => Ok(await _mediator.Send(new RefreshTokenCommand(refreshToken), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> ChangeEmailOrMobile([FromBody] ChangeEmailOrMobileRequest changeEmailOrMobileRequest, CancellationToken token)
         => Ok(await _mediator.Send(new ChangeEmailOrMobileCommand(changeEmailOrMobileRequest), token));

    [HttpPost("[action]"), AllowAnonymous, Produces(typeof(CommitResult))]
    public async Task<IActionResult> ConfirmChangeEmailOrMobile([FromBody] OTPVerificationRequest OTPVerification, CancellationToken token)
      => Ok(await _mediator.Send(new ConfirmChangeEmailOrMobileCommand(OTPVerification), token));


    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changeEmailOrMobileRequest, CancellationToken token)
         => Ok(await _mediator.Send(new ChangePasswordCommand(changeEmailOrMobileRequest), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> VerifyEmail([FromBody] OTPVerificationRequest OTPVerification, CancellationToken token)
         => Ok(await _mediator.Send(new EmailVerificationCommand(OTPVerification), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> AddExternalProvider([FromBody] AddExternalIdentityProviderRequest addExternalIdentityProviderRequest, CancellationToken token)
         => Ok(await _mediator.Send(new AddExternalIdentityProviderCommand(addExternalIdentityProviderRequest), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> RemoveExternalProvider([FromBody] RemoveExternalIdentityProviderRequest removeExternalIdentityProviderRequest, CancellationToken token)
         => Ok(await _mediator.Send(new RemoveExternalIdentityProviderCommand(removeExternalIdentityProviderRequest), token));

    [HttpPost("[action]"), AllowAnonymous, Produces(typeof(CommitResult))]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest forgetPasswordRequest, CancellationToken token)
         => Ok(await _mediator.Send(new ForgetPasswordCommand(forgetPasswordRequest), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> VerifyMobile([FromBody] OTPVerificationRequest OTPVerification, CancellationToken token)
         => Ok(await _mediator.Send(new MobileVerificationCommand(OTPVerification), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult)), AllowAnonymous]
    public async Task<IActionResult> ResendEmailOTP(CancellationToken token)
         => Ok(await _mediator.Send(new ResendEmailVerificationCommand(), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult)), AllowAnonymous]
    public async Task<IActionResult> ResendMobileOTP(CancellationToken token)
         => Ok(await _mediator.Send(new ResendMobileVerificationCommand(), token));

    [HttpPost("[action]"), AllowAnonymous, Produces(typeof(CommitResult<string>))]
    public async Task<IActionResult> ConfirmForgetPassword([FromBody] OTPVerificationRequest OTPVerification, CancellationToken token)
         => Ok(await _mediator.Send(new ConfirmForgetPasswordCommand(OTPVerification), token));



    [HttpPost("[action]"), AllowAnonymous, Produces(typeof(CommitResult))]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest, CancellationToken token)
         => Ok(await _mediator.Send(new ResetPasswordCommand(resetPasswordRequest), token));

    [HttpGet("[action]"), Produces(typeof(CommitResult<LimitedProfileResponse>)), AllowAnonymous]
    public async Task<IActionResult> GetIdentityLimitedProfile([FromQuery(Name = "IdentityId")] string IdentityId, CancellationToken token)
     => Ok(await _mediator.Send(new GetIdentityLimitedProfileQuery(IdentityId), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<LimitedProfileResponse>))]
    public async Task<IActionResult> GetTeacherLimitedProfilesByNameOrMobile([FromQuery(Name = "NameOrMobile")] string NameOrMobile, CancellationToken token)
                 => Ok(await _mediator.Send(new GetTeacherLimitedProfilesByNameOrMobileQuery(NameOrMobile), token));
    [HttpPost("[action]"), Produces(typeof(CommitResults<LimitedProfileResponse>))]
    public async Task<IActionResult> GetIdentityLimitedProfiles([FromBody] List<string> IdentityIds, CancellationToken token)
             => Ok(await _mediator.Send(new GetIdentityLimitedProfilesQuery(IdentityIds), token));

    [HttpGet("[action]"), Produces(typeof(CommitResult<int>))]
    public async Task<IActionResult> GetIdentityGrade([FromQuery(Name = "IdentityId")] string? IdentityId, CancellationToken token)
          => Ok(await _mediator.Send(new GetIdentityGradeQuery(IdentityId), token));
}