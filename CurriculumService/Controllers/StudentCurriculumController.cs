﻿using CurriculumDomain.Features.GetStudentCurriculum.CQRS.Query;
using CurriculumDomain.Features.GetStudentRecentLessonsProgress.CQRS.Query;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurriculumService.Controllers;

[ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class StudentCurriculumController : ControllerBase
{
    private readonly IMediator _mediator;
    public StudentCurriculumController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetStudentCurriculums(CancellationToken token)
         => Ok(await _mediator.Send(new GetStudentCurriculumQuery(), token));

    [HttpGet("[action]")]
    public async Task<IActionResult> GetStudentRecentLessonsProgress(CancellationToken token)
       => Ok(await _mediator.Send(new GetStudentRecentLessonsProgressQuery(), token));
}