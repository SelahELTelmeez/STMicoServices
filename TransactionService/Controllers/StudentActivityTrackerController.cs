using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionDomain.Features.Activities.CQRS.Command;
using TransactionDomain.Features.Activities.DTO.Command;
using TransactionDomain.Features.ClipActivity.CQRS.Query;
using TransactionDomain.Features.IdentityScores.IdentityClipScore.CQRS.Query;
using TransactionDomain.Features.IdentityScores.IdentitySubjectScore.CQRS;
using TransactionDomain.Features.Invitations.IdentityInvitation.CQRS.Query;
using TransactionDomain.Features.StudentRecentLessonsProgress.CQRS.Query;
using TransactionDomain.Features.Tracker.CQRS.Command;
using TransactionDomain.Features.Tracker.DTO.Command;

namespace TransactionService.Controllers
{
    [ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StudentActivityTrackerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentActivityTrackerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> InsertStudentActivity([FromBody] InsertActivityRequest activityrRequest, CancellationToken token)
            => Ok(await _mediator.Send(new InsertActivityCommand(activityrRequest), token));

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateStudentActivity([FromBody] UpdateActivityRequest activityrRequest, CancellationToken token)
            => Ok(await _mediator.Send(new UpdateActivityCommand(activityrRequest), token));

        [HttpGet("[action]")]
        public async Task<IActionResult> GetRecentLessonProgress(CancellationToken token)
           => Ok(await _mediator.Send(new GetStudentRecentLessonsProgressQuery(), token));

        [HttpGet("[action]")]
        public async Task<IActionResult> GetIdentitySubjectScore([FromQuery(Name = "SubjectId")] string SubjectId, CancellationToken token)
            => Ok(await _mediator.Send(new GetIdentitySubjectScoreQuery(SubjectId), token));

        [HttpGet("[action]")]
        public async Task<IActionResult> GetIdentityInvitations(CancellationToken token)
            => Ok(await _mediator.Send(new GetIdentityInvitationsQuery(), token));

        [HttpGet("[action]")]
        public async Task<IActionResult> GetIdentityClipsScore([FromQuery(Name = "LessonId")] int LessonId, CancellationToken token)
            => Ok(await _mediator.Send(new GetIdentityClipsScoreQuery(LessonId), token));

        [HttpPost("[action]")]
        public async Task<IActionResult> GetClipActivities(List<int> ClipIds, CancellationToken token)
            => Ok(await _mediator.Send(new GetClipActivityQuery(ClipIds), token));
    }
}
