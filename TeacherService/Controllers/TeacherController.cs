using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResultHandler;
using SharedModule.DTO;
using TeacherDomain.Features.Assignment.CQRS.Command;
using TeacherDomain.Features.Assignment.CQRS.Query;
using TeacherDomain.Features.Assignment.DTO.Command;
using TeacherDomain.Features.Assignment.DTO.Query;
using TeacherDomain.Features.Classes.CQRS.Command;
using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Command;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherDomain.Features.Quiz.Command.DTO;
using TeacherDomain.Features.Quiz.CQRS.Command;
using TeacherDomain.Features.Quiz.CQRS.Query;
using TeacherDomain.Features.Quiz.DTO.Command;
using TeacherDomain.Features.Quiz.DTO.Query;
using TeacherDomain.Features.TeacherClass.DTO.Command;
using TeacherDomain.Features.TeacherClass.DTO.Query;
using TeacherDomain.Features.TeacherSubject.CQRS.Command;
using TeacherDomain.Features.TeacherSubject.CQRS.Query;
using TeacherDomain.Features.Tracker.CQRS.Query;
using TeacherDomain.Features.Tracker.DTO.Query;

namespace TeacherService.Controllers;

[ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TeacherController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeacherController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("[action]"), Produces(typeof(CommitResult<int>))]
    public async Task<IActionResult> CreateClass([FromBody] CreateClassRequest createClassRequest, CancellationToken token)
       => Ok(await _mediator.Send(new CreateClassCommand(createClassRequest), token));

    [HttpPut("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> UpdateClass([FromBody] UpdateClassRequest updateClassRequest, CancellationToken token)
       => Ok(await _mediator.Send(new UpdateClassCommand(updateClassRequest), token));

    [HttpGet("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> ToggleActivateClass([FromQuery(Name = "ClassId")] int ClassId, CancellationToken token)
       => Ok(await _mediator.Send(new ToggleActivateClassCommand(ClassId), token));

    [HttpGet("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> RequestToEnrollClass([FromQuery(Name = "ClassId")] int ClassId, CancellationToken token)
       => Ok(await _mediator.Send(new RequestEnrollToClassCommand(ClassId), token));

    [HttpGet("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> UnrollFromClass([FromQuery(Name = "ClassId")] int ClassId, CancellationToken token)
       => Ok(await _mediator.Send(new UnrollFromClassCommand(ClassId), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> UnrollStudentFromClassByTeacher([FromBody] UnrollStudentFromClassByTeacherRequest request, CancellationToken token)
       => Ok(await _mediator.Send(new UnrollStudentFromClassByTeacherCommand(request), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> AcceptStudentEnrollToClassRequest([FromBody] AcceptStudentEnrollToClassRequest AcceptStudentEnrollToClassRequest, CancellationToken token)
       => Ok(await _mediator.Send(new AcceptStudentEnrollToClassRequestCommand(AcceptStudentEnrollToClassRequest), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentRequest createAssignmentRequest, CancellationToken token)
       => Ok(await _mediator.Send(new CreateAssignmentCommand(createAssignmentRequest), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> ReplyAssignment([FromBody] ReplyAssignmentRequest replyAssignmentRequest, CancellationToken token)
       => Ok(await _mediator.Send(new ReplyAssignmentCommand(replyAssignmentRequest), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<AssignmentResponse>))]
    public async Task<IActionResult> GetAssignments(CancellationToken token)
       => Ok(await _mediator.Send(new GetAssignmentsQuery(), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<QuizResponse>))]
    public async Task<IActionResult> GetQuizzes(CancellationToken token)
       => Ok(await _mediator.Send(new GetQuizzesQuery(), token));

    [HttpGet("[action]"), Produces(typeof(CommitResult<bool>))]
    public async Task<IActionResult> CheckAnyClassExistenceBySubjectId([FromQuery(Name = "SubjectId")] string SubjectId, CancellationToken token)
       => Ok(await _mediator.Send(new CheckAnyClassExistenceBySubjectIdQuery(SubjectId), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<ClassActivityResponse>))]
    public async Task<IActionResult> GetActivitiesByClass([FromQuery(Name = "ClassId")] int ClassId, CancellationToken token)
       => Ok(await _mediator.Send(new GetActivitiesByClassQuery(ClassId), token));


    [HttpGet("[action]"), Produces(typeof(CommitResults<ClassBriefResponse>))]
    public async Task<IActionResult> GetClassesByQuizId([FromQuery(Name = "ClassId")] int ClassId, CancellationToken token)
       => Ok(await _mediator.Send(new GetClassesByQuizIdQuery(ClassId), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<StudentClassActivityResponse>))]
    public async Task<IActionResult> GetStudentClassActivitiesByClass([FromQuery(Name = "ClassId")] int ClassId, [FromQuery(Name = "StudentId")] Guid StudentId, CancellationToken token)
       => Ok(await _mediator.Send(new GetStudentClassActivityQuery(StudentId, ClassId), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<EnrolledStudentResponse>))]
    public async Task<IActionResult> GetEnrolledStudentsByClass([FromQuery(Name = "ClassId")] int ClassId, CancellationToken token)
       => Ok(await _mediator.Send(new GetEnrolledStudentsByClassQuery(ClassId), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<StudentClassResponse>))]
    public async Task<IActionResult> GetStudentClasses([FromQuery(Name = "StudentId")] Guid? StudentId, CancellationToken token)
       => Ok(await _mediator.Send(new GetStudentClassesQuery(StudentId), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<TeacherClassResponse>))]
    public async Task<IActionResult> GetTeacherClassesBySubject([FromQuery(Name = "SubjectId")] string SubjectId, CancellationToken token)
       => Ok(await _mediator.Send(new GetTeacherClassesBySubjectQuery(SubjectId), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<TeacherClassResponse>))]
    public async Task<IActionResult> GetTeacherClasses(CancellationToken token)
       => Ok(await _mediator.Send(new GetTeacherClassesQuery(), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<ClassResponse>))]
    public async Task<IActionResult> SearchClassBySubject([FromQuery(Name = "SubjectId")] string SubjectId, CancellationToken token)
       => Ok(await _mediator.Send(new SearchClassBySubjectQuery(SubjectId), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<ClassResponse>))]
    public async Task<IActionResult> SearchClassByTeacher([FromQuery(Name = "NameOrMobile")] string NameOrMobile, CancellationToken token)
       => Ok(await _mediator.Send(new SearchClassByTeacherQuery(NameOrMobile), token));

    [HttpGet("[action]"), Produces(typeof(CommitResult<ClassResponse>))]
    public async Task<IActionResult> SearchClass([FromQuery(Name = "ClassId")] int ClassId, CancellationToken token)
       => Ok(await _mediator.Send(new SearchClassQuery(ClassId), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizRequest CreateQuizRequest, CancellationToken token)
       => Ok(await _mediator.Send(new CreateQuizCommand(CreateQuizRequest), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> ReplyQuiz([FromBody] ReplyQuizRequest ReplyQuizRequest, CancellationToken token)
       => Ok(await _mediator.Send(new ReplyQuizCommand(ReplyQuizRequest), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> AddSubject([FromBody] IEnumerable<string> SubjectIds, CancellationToken token)
       => Ok(await _mediator.Send(new AddTeacherSubjectCommand(SubjectIds), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<TeacherSubjectResponse>))]
    public async Task<IActionResult> GetSubjects(CancellationToken token)
       => Ok(await _mediator.Send(new GetTeacherSubjectQuery(), token));

    [HttpGet("[action]"), Produces(typeof(CommitResult<EnrolleeDetailsResponse>))]
    public async Task<IActionResult> GetEnrolleeDetails([FromQuery(Name = "EnrolleeId")] Guid EnrolleeId, CancellationToken token)
       => Ok(await _mediator.Send(new GetEnrolleeDetailsQuery(EnrolleeId), token));


    [HttpGet("[action]"), Produces(typeof(CommitResults<LimitedTeacherProfileResponse>))]
    public async Task<IActionResult> GetTeachersByStudentId([FromQuery(Name = "StudentId")] Guid StudentId, CancellationToken token)
       => Ok(await _mediator.Send(new GetTeachersByStudentIdQuery(StudentId), token));


    [HttpPost("[action]"), Produces(typeof(CommitResult<TeacherClassesByStudentResponse>))]
    public async Task<IActionResult> GetTeacherClassesByStudent(TeacherClassesByStudentRequest Request, CancellationToken token)
       => Ok(await _mediator.Send(new GetTeacherClassesByStudentQuery(Request), token));


    [HttpGet("[action]"), Produces(typeof(CommitResults<EnrolledStudentAssignmentResponse>))]
    public async Task<IActionResult> GetEnrolledStudentsByAssignmentActivity([FromQuery(Name = "ClassId")] int ClassId,
        [FromQuery(Name = "AssignmentId")] int AssignmentId,
        CancellationToken token)
       => Ok(await _mediator.Send(new GetEnrolledStudentsByAssignmentActivityIdQuery(ClassId, AssignmentId), token));


    [HttpGet("[action]"), Produces(typeof(CommitResults<EnrolledStudentQuizResponse>))]
    public async Task<IActionResult> GetEnrolledStudentsByQuizActivity([FromQuery(Name = "ClassId")] int ClassId, [FromQuery(Name = "QuizId")] int QuizId, CancellationToken token)
       => Ok(await _mediator.Send(new GetEnrolledStudentsByQuizActivityIdQuery(ClassId, QuizId), token));


}
