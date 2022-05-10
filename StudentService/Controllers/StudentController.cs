﻿using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResultHandler;
using StudentDomain.Features.Activities.CQRS.Command;
using StudentDomain.Features.Activities.DTO.Command;
using StudentDomain.Features.IdentityScores.IdentityClipScore.CQRS.Query;
using StudentDomain.Features.IdentityScores.IdentityClipScore.DTO;
using StudentDomain.Features.IdentityScores.IdentitySubjectScore.CQRS;
using StudentDomain.Features.IdentityScores.IdentitySubjectScore.DTO;
using StudentDomain.Features.Tracker.CQRS.Command;
using StudentDomain.Features.Tracker.CQRS.Query;
using StudentDomain.Features.Tracker.DTO;
using StudentDomain.Features.Tracker.DTO.Command;
using StudentDomain.Features.Tracker.DTO.Query;

namespace StudentService.Controllers;

[ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class StudentController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("[action]"), Produces(typeof(CommitResult<int>))]
    public async Task<IActionResult> InsertStudentActivity([FromBody] InsertActivityRequest activityrRequest, CancellationToken token)
        => Ok(await _mediator.Send(new InsertActivityCommand(activityrRequest), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> UpdateStudentActivity([FromBody] UpdateActivityRequest activityrRequest, CancellationToken token)
        => Ok(await _mediator.Send(new UpdateActivityCommand(activityrRequest), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<StudentRecentLessonProgressResponse>))]
    public async Task<IActionResult> GetRecentLessonProgress(CancellationToken token)
       => Ok(await _mediator.Send(new GetStudentRecentLessonsProgressQuery(), token));

    [HttpGet("[action]"), Produces(typeof(CommitResult<IdentitySubjectScoreResponse>))]
    public async Task<IActionResult> GetIdentitySubjectScore([FromQuery(Name = "SubjectId")] string SubjectId, CancellationToken token)
        => Ok(await _mediator.Send(new GetIdentitySubjectScoreQuery(SubjectId), token));

    [HttpGet("[action]"), Produces(typeof(CommitResult<IdentityClipsScoreResponse>))]
    public async Task<IActionResult> GetIdentityClipsScore([FromQuery(Name = "LessonId")] int LessonId, CancellationToken token)
        => Ok(await _mediator.Send(new GetIdentityClipsScoreQuery(LessonId), token));

    [HttpPost("[action]"), Produces(typeof(CommitResults<ClipActivityResponse>))]
    public async Task<IActionResult> GetClipActivities(List<int> ClipIds, CancellationToken token)
        => Ok(await _mediator.Send(new GetClipActivityQuery(ClipIds), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> SubmitStudentQuizAnswer(UpdateStudentQuizRequest studentQuizRequest, CancellationToken token)
        => Ok(await _mediator.Send(new UpdateStudentQuizCommand(studentQuizRequest), token));

    [HttpPost("[action]"), Produces(typeof(CommitResults<StudentQuizResultResponse>))]
    public async Task<IActionResult> GetStudentQuizResults(StudentQuizResultRequest studentQuizResult, CancellationToken token)
        => Ok(await _mediator.Send(new GetStudentQuizzesResultQuery(studentQuizResult), token));
}