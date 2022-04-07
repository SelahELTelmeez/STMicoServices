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
        public async Task<IActionResult> GetSubjectUnits([FromQuery(Name = "SubjectId")] string SubjectId, CancellationToken token)
             => Ok(await _mediator.Send(new GetSubjectUnitsQuery(SubjectId), token));

        [HttpGet("[action]")]
        public async Task<IActionResult> GetLessonClips([FromQuery(Name = "LessonId")] int LessonId, CancellationToken token)
             => Ok(await _mediator.Send(new GetLessonClipQuery(LessonId), token));

        [HttpGet("[action]")]
        public async Task<IActionResult> GetLessonDetails([FromQuery(Name = "LessonId")] int LessonId, CancellationToken token)
            => Ok(await _mediator.Send(new GetLessonDetailsByIdQuery(LessonId), token));

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSubjectLessonScores([FromQuery(Name = "SubjectId")] string SubjectId, CancellationToken token)
            => Ok(await _mediator.Send(new GetSubjectLessonScoresQuery(SubjectId), token));

        [HttpGet("[action]")]
        public async Task<IActionResult> GetLessonClipScores([FromQuery(Name = "LessonId")] int LessonId, CancellationToken token)
            => Ok(await _mediator.Send(new GetLessonClipScoresQuery(LessonId), token));
    }
}
