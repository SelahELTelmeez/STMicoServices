using CurriculumDomain.Features.LessonClip.CQRS.Query;
using CurriculumDomain.Features.SubjectUnit.CQRS.Query;
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
        public async Task<IActionResult> GetCurriculumUnits([FromQuery(Name = "SubjectId")] string SubjectId, CancellationToken token)
             => Ok(await _mediator.Send(new GetSubjectUnitsQuery(SubjectId), token));

        [HttpGet("[action]")]
        public async Task<IActionResult> GetLessonClips([FromQuery(Name = "LessonId")] int LessonId, CancellationToken token)
             => Ok(await _mediator.Send(new GetLessonClipQuery(LessonId), token));
    }
}
