using Flaminco.CommitResult;
using IdentityDomain.Features.ResendEmailVerification.CQRS.Command;
using IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
using IdentityDomain.Models;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace IdentityInfrastructure.Features.ResendEmailVerification.CQRS.Command;
public class ResendEmailVerificationCommandHandler : IRequestHandler<ResendEmailVerificationCommand, ICommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly INotificationService _notificationEmailService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMediator _mediator;

    public ResendEmailVerificationCommandHandler(STIdentityDbContext dbContext,
                                                 INotificationService notificationEmailService,
                                                 IWebHostEnvironment configuration,
                                                 IHttpContextAccessor httpContextAccessor, IMediator mediator)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _notificationEmailService = notificationEmailService;
        _httpContextAccessor = httpContextAccessor;
        _mediator = mediator;
    }

    public async Task<ICommitResult> Handle(ResendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _mediator.Send(new GetIdentityUserByIdQuery(_httpContextAccessor.GetIdentityUserId()), cancellationToken);

        if (identityUser == null)
        {
            return ResultType.NotFound.GetCommitResult("XIDN0001", _resourceJsonManager["XIDN0001"]);
        }
        else
        {
            //2.0 Check if email is null
            if (identityUser.Email == null)
            {
                return ResultType.NotFound.GetCommitResult("XIDN0012", _resourceJsonManager["XIDN0012"]);
            }

            //3.0 Disable All Previous Resend Email Verification Code.
            await _dbContext.Entry(identityUser).Collection(a => a.Activations).LoadAsync(cancellationToken);

            if (identityUser.Activations.Where(a => a.IsActive && a.ActivationType == ActivationType.Email).Any())
            {
                foreach (IdentityActivation activation in identityUser.Activations)
                {
                    activation.RevokedOn = DateTime.UtcNow;
                    _dbContext.Set<IdentityActivation>().Update(activation);
                }
            }
            IdentityActivation identityActivation = new IdentityActivation
            {
                ActivationType = ActivationType.Email,
                Code = UtilityGenerator.GetOTP(4).ToString(),
                IdentityUserId = identityUser.Id,
            };
            _dbContext.Set<IdentityActivation>().Add(identityActivation);
            await _dbContext.SaveChangesAsync(cancellationToken);

            bool result = await _notificationEmailService.SendEmailAsync(new EmailNotificationModel
            {
                MailFrom = "noreply@selaheltelmeez.com",
                MailTo = identityUser.Email,
                MailSubject = "سلاح التلميذ - رمز التفعيل",
                IsBodyHtml = true,
                DisplayName = "سلاح التلميذ",
                MailToName = identityUser.FullName,
                MailBody = identityActivation.Code
            }, cancellationToken);

            if (result)
            {
                return ResultType.Ok.GetCommitResult();
            }
            else
            {
                return ResultType.Invalid.GetCommitResult("XIDN0013", _resourceJsonManager["XIDN0013"]);
            }
        }
    }
}