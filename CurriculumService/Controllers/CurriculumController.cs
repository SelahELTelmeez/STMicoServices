using CurriculumDomain.Features.Lessons.GetClipsBrief.CQRS.Query;
using CurriculumDomain.Features.Lessons.GetLessonClips.CQRS.Query;
using CurriculumDomain.Features.Lessons.GetLessonClips.DTO.Query;
using CurriculumDomain.Features.Lessons.GetLessonDetails.CQRS.Query;
using CurriculumDomain.Features.Lessons.GetLessonDetails.DTO.Query;
using CurriculumDomain.Features.Lessons.GetLessonsBrief.CQRS.Query;
using CurriculumDomain.Features.Quizzes.CQRS.Command;
using CurriculumDomain.Features.Quizzes.CQRS.Query;
using CurriculumDomain.Features.Quizzes.DTO.Command;
using CurriculumDomain.Features.Quizzes.DTO.Query;
using CurriculumDomain.Features.Reports.CQRS.Query;
using CurriculumDomain.Features.Subjects.CQRS.Query;
using CurriculumDomain.Features.Subjects.DTO;
using CurriculumDomain.Features.Subjects.GetLessonsBrief.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetLessonsBrief.DTO.Query;
using CurriculumDomain.Features.Subjects.GetStudentSubjects.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetStudentSubjects.DTO.Query;
using CurriculumDomain.Features.Subjects.GetSubjectBrief.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetSubjects.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetSubjectUnits.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetTeacherSubjects.CQRS.Query;
using CurriculumDomain.Features.Subjects.VerifySubjectStudentGradeMatching.CQRS.Query;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResultHandler;
using SharedModule.DTO;

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

        [HttpGet("[action]"), Produces(typeof(CommitResult<UnitResponse>))]
        public async Task<IActionResult> GetUnits([FromQuery(Name = "SubjectId")] string SubjectId, CancellationToken token)
             => Ok(await _mediator.Send(new GetUnitsBySubjectIdQuery(SubjectId), token));

        [HttpGet("[action]"), Produces(typeof(CommitResult<LessonClipResponse>))]
        public async Task<IActionResult> GetClips([FromQuery(Name = "LessonId")] int LessonId, CancellationToken token)
             => Ok(await _mediator.Send(new GetLessonClipQuery(LessonId), token));

        [HttpGet("[action]"), Produces(typeof(CommitResult<LessonDetailsReponse>))]
        public async Task<IActionResult> GetLessonDetails([FromQuery(Name = "LessonId")] int LessonId, CancellationToken token)
            => Ok(await _mediator.Send(new GetLessonDetailsQuery(LessonId), token));

        [HttpPost("[action]"), Produces(typeof(CommitResults<LessonBriefResponse>))]
        public async Task<IActionResult> GetLessonsBrief([FromBody] List<int> LessonIds, CancellationToken token)
         => Ok(await _mediator.Send(new GetLessonsBriefQuery(LessonIds), token));

        [HttpGet("[action]"), Produces(typeof(CommitResults<LessonsBriefResponse>))]
        public async Task<IActionResult> GetLessonsBriefBySubject([FromQuery(Name = "SubjectId")] string SubjectId, CancellationToken token)
            => Ok(await _mediator.Send(new GetLessonsBriefBySubjectIdQuery(SubjectId), token));

        [HttpGet("[action]"), Produces(typeof(CommitResults<ClipBriefResponse>))]
        public async Task<IActionResult> GetClipsBrief([FromQuery(Name = "LessonId")] int LessonId, CancellationToken token)
            => Ok(await _mediator.Send(new GetClipsBriefByLessonIdQuery(LessonId), token));

        [HttpGet("[action]"), Produces(typeof(CommitResult<SubjectBriefResponse>))]
        public async Task<IActionResult> GetSubjectBrief([FromQuery(Name = "SubjectId")] string SubjectId, CancellationToken token)
          => Ok(await _mediator.Send(new GetSubjectBriefQuery(SubjectId), token));

        [HttpPost("[action]"), Produces(typeof(CommitResults<SubjectBriefResponse>))]
        public async Task<IActionResult> GetSubjectsBrief(IEnumerable<string> SubjectIds, CancellationToken token)
            => Ok(await _mediator.Send(new GetSubjectsBriefQuery(SubjectIds), token));


        [HttpPost("[action]"), Produces(typeof(CommitResults<SubjectResponse>))]
        public async Task<IActionResult> GetSubjects([FromQuery(Name = "SubjectIds")] IEnumerable<string> SubjectIds, CancellationToken token)
         => Ok(await _mediator.Send(new GetSubjectsQuery(SubjectIds), token));


        [HttpGet("[action]"), Produces(typeof(CommitResult<bool>))]
        public async Task<IActionResult> VerifySubjectGradeMatching([FromQuery(Name = "SubjectId")] string SubjectId, [FromQuery(Name = "GradeId")] int GradeId, CancellationToken token)
                 => Ok(await _mediator.Send(new VerifySubjectStudentGradeMatchingQuery(SubjectId, GradeId), token));


        [HttpGet("[action]"), Produces(typeof(CommitResult<bool>))]
        public async Task<IActionResult> CheckAnyMCQExistenceBySubjectId([FromQuery(Name = "SubjectId")] string SubjectId, CancellationToken token)
                => Ok(await _mediator.Send(new CheckAnyMCQExistenceBySubjectIdQuery(SubjectId), token));

        [HttpPost("[action]"), Produces(typeof(CommitResults<TeacherSubjectResponse>))]
        public async Task<IActionResult> GetTeacherSubjects([FromBody] List<string> SubjectIds, CancellationToken token)
            => Ok(await _mediator.Send(new GetTeacherSubjectsQuery(SubjectIds), token));

        [HttpGet("[action]"), Produces(typeof(CommitResults<IdnentitySubjectResponse>))]
        public async Task<IActionResult> GetStudentSubjects([FromQuery(Name = "StudentId")] Guid? StudentId, CancellationToken token)
           => Ok(await _mediator.Send(new GetStudentSubjectsQuery(StudentId), token));


        [HttpGet("[action]"), Produces(typeof(CommitResult<DetailedProgressResponse>))]
        public async Task<IActionResult> GetSubjectDetailedProgress([FromQuery(Name = "SubjectId")] string SubjectId, CancellationToken token)
            => Ok(await _mediator.Send(new GetSubjectDetailedProgressQuery(SubjectId), token));


        [HttpPost("[action]"), Produces(typeof(CommitResults<SubjectDetailedResponse>))]
        public async Task<IActionResult> GetSubjectsDetailed(IEnumerable<string> SubjectIds, CancellationToken token)
            => Ok(await _mediator.Send(new GetSubjectsDetailedQuery(SubjectIds), token));


        [HttpGet("[action]"), Produces(typeof(CommitResults<SubjectBriefProgressResponse>))]
        public async Task<IActionResult> GetSubjectsBriefProgress([FromQuery(Name = "Term")] int Term, [FromQuery(Name = "StudentId")] Guid? StudentId, CancellationToken token)
         => Ok(await _mediator.Send(new GetSubjectsBriefProgressQuery(Term, StudentId), token));


        [HttpPost("[action]"), Produces(typeof(CommitResult<int>))]
        public async Task<IActionResult> CreateQuize([FromBody] int clipId, CancellationToken token)
             => Ok(await _mediator.Send(new CreateQuizCommand(clipId), token));

        [HttpPost("[action]"), Produces(typeof(CommitResult))]
        public async Task<IActionResult> SubmitQuiz([FromBody] UserQuizAnswersRequest UserQuizAnswersRequest, CancellationToken token)
           => Ok(await _mediator.Send(new SubmitQuizAnswerCommand(UserQuizAnswersRequest), token));


        [HttpGet("[action]"), Produces(typeof(CommitResults<SubjectProfileResponse>))]
        public async Task<IActionResult> GetSubjectsByFilters([FromQuery(Name = "Grade")] int Grade, [FromQuery(Name = "TermId")] int Term, CancellationToken token)
            => Ok(await _mediator.Send(new GetSubjectsByFiltersQuery(Grade, Term), token));


        [HttpGet("[action]"), Produces(typeof(CommitResults<SubjectProfileResponse>))]
        public async Task<IActionResult> GetSubjectsBriefByTerm([FromQuery(Name = "Grade")] int Grade, [FromQuery(Name = "Term")] int Term, CancellationToken token)
            => Ok(await _mediator.Send(new GetSubjectsBriefByTermQuery(Grade, Term), token));


        [HttpGet("[action]"), Produces(typeof(CommitResult<QuizDetailsResponse>))]
        public async Task<IActionResult> GetQuizDetails([FromQuery(Name = "QuizId")] int QuizId, CancellationToken token)
            => Ok(await _mediator.Send(new GetQuizDetailsQuery(QuizId), token));


        [HttpGet("[action]"), Produces(typeof(CommitResult<QuizDetailsResponse>))]
        public async Task<IActionResult> GetStudentQuizAttempts([FromQuery(Name = "QuizId")] int QuizId, [FromQuery(Name = "StudentId")] Guid StudentId, CancellationToken token)
               => Ok(await _mediator.Send(new GetStudentAttemptsQuery(StudentId, QuizId), token));

    }
}