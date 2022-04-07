using CurriculumDomain.Features.Lessons.GetClipsBrief.CQRS.Query;
using CurriculumDomain.Features.Lessons.GetLessonClips.CQRS.Query;
using CurriculumDomain.Features.Lessons.GetLessonDetails.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetLessonsBrief.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetSubjectUnits.CQRS.Query;
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
        public async Task<IActionResult> GetUnits([FromQuery(Name = "SubjectId")] string SubjectId, CancellationToken token)
             => Ok(await _mediator.Send(new GetUnitsBySubjectIdQuery(SubjectId), token));

        [HttpGet("[action]")]
        public async Task<IActionResult> GetClips([FromQuery(Name = "LessonId")] int LessonId, CancellationToken token)
             => Ok(await _mediator.Send(new GetLessonClipQuery(LessonId), token));

        [HttpGet("[action]")]
        public async Task<IActionResult> GetLessonDetails([FromQuery(Name = "LessonId")] int LessonId, CancellationToken token)
            => Ok(await _mediator.Send(new GetLessonDetailsQuery(LessonId), token));

        [HttpGet("[action]")]
        public async Task<IActionResult> GetLessonsBrief([FromQuery(Name = "SubjectId")] string SubjectId, CancellationToken token)
            => Ok(await _mediator.Send(new GetLessonsBriefBySubjectIdQuery(SubjectId), token));

        [HttpGet("[action]")]
        public async Task<IActionResult> GetClipsBrief([FromQuery(Name = "LessonId")] int LessonId, CancellationToken token)
            => Ok(await _mediator.Send(new GetClipsBriefByLessonIdQuery(LessonId), token));
    }
}