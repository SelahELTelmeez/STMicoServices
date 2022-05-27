using Flaminco.CommitResult;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.CQRS.Command;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Invitations;
using SharedModule.Extensions;

namespace NotifierInfrastructure.Features.CQRS.Command
{
    public class SetAsInactiveInvitationCommandHandler : IRequestHandler<SetAsInactiveInvitationCommand, ICommitResult>
    {
        private readonly NotifierDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;
        private readonly IMediator _mediator;
        public SetAsInactiveInvitationCommandHandler(NotifierDbContext dbContext,
                                              IWebHostEnvironment configuration,
                                              IHttpContextAccessor httpContextAccessor,
                                              IMediator mediator)
        {
            _dbContext = dbContext;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
            _mediator = mediator;
        }

        public async Task<ICommitResult> Handle(SetAsInactiveInvitationCommand request, CancellationToken cancellationToken)
        {
            Invitation? invitation = await _dbContext.Set<Invitation>().SingleOrDefaultAsync(a => a.Id.Equals(request.InvitationId), cancellationToken);

            if (invitation == null)
            {
                return Flaminco.CommitResult.ResultType.NotFound.GetCommitResult("X0000", _resourceJsonManager["X0000"]);
            }

            invitation.IsActive = false;
            _dbContext.Set<Invitation>().Update(invitation);

            //await _mediator.Send(new SendNotificationCommand(new NotificationRequest
            //{
            //    NotificationTypeId = 1,
            //    NotifiedId = invitation.InviterId,
            //    NotifierId = invitation.InvitedId,
            //    Argument = invitation.Argument,
            //}));


            return Flaminco.CommitResult.ResultType.Ok.GetCommitResult();

        }
    }
}
