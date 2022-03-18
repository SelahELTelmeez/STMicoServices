using CurriculumDomain.Features.GetCurriculumLesson.CQRS.Query;
using CurriculumDomain.Features.GetCurriculumUnit.CQRS.Query;
using CurriculumDomain.Features.GetStudentCurriculum.CQRS.Query;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurriculumService.Controllers
{
    [ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CurriculumController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CurriculumController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetStudentCurriculums(CancellationToken token)
             => Ok(await _mediator.Send(new GetStudentCurriculumQuery(), token));

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCurriculumUnits([FromQuery(Name = "CurriculumId")] string CurriculumId, CancellationToken token)
             => Ok(await _mediator.Send(new GetCurriculumUnitQuery(CurriculumId), token));

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCurriculumLessons([FromQuery(Name = "LessonId")] int LessonId, CancellationToken token)
             => Ok(await _mediator.Send(new GetCurriculumLessonQuery(LessonId), token));
    }
}
