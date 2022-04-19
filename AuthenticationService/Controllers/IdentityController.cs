using IdentityDomain.Features.ChangeEmailOrMobile.CQRS.Command;
using IdentityDomain.Features.ChangeEmailOrMobile.DTO.Command;
using IdentityDomain.Features.ChangePassword.CQRS.Command;
using IdentityDomain.Features.ChangePassword.DTO.Command;
using IdentityDomain.Features.ConfirmForgetPassword.CQRS.Command;
using IdentityDomain.Features.EmailVerification.CQRS.Command;
using IdentityDomain.Features.ExternalIdentityProvider.CQRS.Add.Command;
using IdentityDomain.Features.ExternalIdentityProvider.CQRS.Command;
using IdentityDomain.Features.ExternalIdentityProvider.DTO.Add.Command;
using IdentityDomain.Features.ExternalIdentityProvider.DTO.Remove.Command;
using IdentityDomain.Features.ForgetPassword.CQRS.Command;
using IdentityDomain.Features.ForgetPassword.DTO.Command;
using IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;
using IdentityDomain.Features.IdentityUserInvitations.CQRS.Query;
using IdentityDomain.Features.IdentityUserTransaction.CQRS.Command;
using IdentityDomain.Features.IdentityUserTransaction.CQRS.Query;
using IdentityDomain.Features.IdentityUserTransaction.DTO;
using IdentityDomain.Features.Login.CQRS.Command;
using IdentityDomain.Features.Login.DTO.Command;
using IdentityDomain.Features.MobileVerification.CQRS.Command;
using IdentityDomain.Features.Refresh.CQRS.Command;
using IdentityDomain.Features.Register.CQRS.Command;
using IdentityDomain.Features.Register.DTO.Command;
using IdentityDomain.Features.ResendEmailVerification.CQRS.Command;
using IdentityDomain.Features.ResendMobileVerification.CQRS.Command;
using IdentityDomain.Features.ResetPassword.CQRS.Command;
using IdentityDomain.Features.ResetPassword.DTO;
using IdentityDomain.Features.Shared.DTO;
using IdentityDomain.Features.UpdateProfile.CQRS.Command;
using IdentityDomain.Features.UpdateProfile.DTO.Command;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class IdentityController : ControllerBase
{
    private readonly IMediator _mediator;

    public IdentityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("[action]"), AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, CancellationToken token)
    {
        return Ok(await _mediator.Send(new LoginCommand(loginRequest), token));
    }

    [HttpPost("[action]"), AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest, CancellationToken token)
         => Ok(await _mediator.Send(new RegisterCommand(registerRequest), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest updateProfileRequest, CancellationToken token)
     => Ok(await _mediator.Send(new UpdateProfileCommand(updateProfileRequest), token));

    [HttpPost("[action]"), AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken, CancellationToken token)
         => Ok(await _mediator.Send(new RefreshTokenCommand(refreshToken), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> ChangeEmailOrMobile([FromBody] ChangeEmailOrMobileRequest changeEmailOrMobileRequest, CancellationToken token)
         => Ok(await _mediator.Send(new ChangeEmailOrMobileCommand(changeEmailOrMobileRequest), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changeEmailOrMobileRequest, CancellationToken token)
         => Ok(await _mediator.Send(new ChangePasswordCommand(changeEmailOrMobileRequest), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> VerifyEmail([FromBody] OTPVerificationRequest OTPVerification, CancellationToken token)
         => Ok(await _mediator.Send(new EmailVerificationCommand(OTPVerification), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> AddExternalProvider([FromBody] AddExternalIdentityProviderRequest addExternalIdentityProviderRequest, CancellationToken token)
         => Ok(await _mediator.Send(new AddExternalIdentityProviderCommand(addExternalIdentityProviderRequest), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> RemoveExternalProvider([FromBody] RemoveExternalIdentityProviderRequest removeExternalIdentityProviderRequest, CancellationToken token)
         => Ok(await _mediator.Send(new RemoveExternalIdentityProviderCommand(removeExternalIdentityProviderRequest), token));

    [HttpPost("[action]"), AllowAnonymous]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest forgetPasswordRequest, CancellationToken token)
         => Ok(await _mediator.Send(new ForgetPasswordCommand(forgetPasswordRequest), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> VerifyMobile([FromBody] OTPVerificationRequest OTPVerification, CancellationToken token)
         => Ok(await _mediator.Send(new MobileVerificationCommand(OTPVerification), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> ResendEmailOTP(CancellationToken token)
         => Ok(await _mediator.Send(new ResendEmailVerificationCommand(), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> ResendMobileOTP(CancellationToken token)
         => Ok(await _mediator.Send(new ResendMobileVerificationCommand(), token));

    [HttpPost("[action]"), AllowAnonymous]
    public async Task<IActionResult> ConfirmForgetPassword([FromBody] OTPVerificationRequest OTPVerification, CancellationToken token)
         => Ok(await _mediator.Send(new ConfirmForgetPasswordCommand(OTPVerification), token));

    [HttpPost("[action]"), AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest, CancellationToken token)
         => Ok(await _mediator.Send(new ResetPasswordCommand(resetPasswordRequest), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> GetIdentityUserInvitations([FromBody] List<Guid> InviterUserIds, CancellationToken token)
         => Ok(await _mediator.Send(new GetIdentityUserInvitationsQuery(InviterUserIds), token));

    [HttpGet("[action]")]
    public async Task<IActionResult> GetIdentityLimitedProfile([FromQuery(Name = "IdentityId")] Guid IdentityId, CancellationToken token)
     => Ok(await _mediator.Send(new GetIdentityLimitedProfileQuery(IdentityId), token));
    [HttpGet("[action]")]
    public async Task<IActionResult> GetTeacherLimitedProfilesByNameOrMobile([FromQuery(Name = "NameOrMobile")] string NameOrMobile, CancellationToken token)
                 => Ok(await _mediator.Send(new GetTeacherLimitedProfilesByNameOrMobileQuery(NameOrMobile), token));
    [HttpPost("[action]")]
    public async Task<IActionResult> GetIdentityLimitedProfiles([FromBody] List<Guid> IdentityIds, CancellationToken token)
             => Ok(await _mediator.Send(new GetIdentityLimitedProfilesQuery(IdentityIds), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> AddNewChild([FromBody] AddNewChildRequest AddNewChildRequest, CancellationToken token)
     => Ok(await _mediator.Send(new AddNewChildCommand(AddNewChildRequest), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> RemoveChild([FromBody] RemoveChildRequest RemoveChildRequest, CancellationToken token)
     => Ok(await _mediator.Send(new RemoveChildCommand(RemoveChildRequest), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> SearchOnStudent([FromBody] SearchOnStudentRequest SearchOnStudentRequest, CancellationToken token)
     => Ok(await _mediator.Send(new SearchOnStudentQuery(SearchOnStudentRequest), token));

    public async Task<IActionResult> GetParentKids( CancellationToken token)
     => Ok(await _mediator.Send(new GetIdentityRelationUserQuery(), token));
}