using CurriculumDomain.Features.Quizzes.Quiz.CQRS.Command;
using CurriculumDomain.Features.Quizzes.Quiz.DTO.Command;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurriculumService.Controllers
{
    [ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class QuizController : ControllerBase
    {
        private readonly IMediator _mediator;
        public QuizController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateQuize([FromBody] QuizRequest quizRequest, CancellationToken token)
             => Ok(await _mediator.Send(new CreateQuizCommand(quizRequest), token));

    }
}