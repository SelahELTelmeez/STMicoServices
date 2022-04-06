﻿using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionDomain.Features.Activities.CQRS.Command;
using TransactionDomain.Features.Activities.DTO.Command;
using TransactionDomain.Features.ClipActivity.CQRS.Query;
using TransactionDomain.Features.IdentityInvitation.CQRS.Query;
using TransactionDomain.Features.IdentitySubjectScore.CQRS;
using TransactionDomain.Features.LessonClipScore.CQRS.Query;
using TransactionDomain.Features.StudentRecentLessonsProgress.CQRS.Query;

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
        public async Task<IActionResult> InsertStudentActivity([FromBody] ActivityRequest activityrRequest, CancellationToken token)
                 => Ok(await _mediator.Send(new InsertActivityCommand(activityrRequest), token));


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

        [HttpGet("[action]")]
        public async Task<IActionResult> GetClipActivities([FromQuery(Name = "ClipIds")] List<int> ClipIds, CancellationToken token)
            => Ok(await _mediator.Send(new GetClipActivityQuery(ClipIds), token));
    }
}
