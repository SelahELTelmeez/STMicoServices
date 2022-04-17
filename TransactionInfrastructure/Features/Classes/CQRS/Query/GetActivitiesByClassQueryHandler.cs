using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Classes.CQRS.Query;
using TransactionDomain.Features.Classes.DTO.Query;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Shared;
using TransactionEntites.Entities.TeacherActivity;
using TransactionEntites.Entities.TeacherClasses;
using TransactionEntites.Entities.Trackers;

namespace TransactionInfrastructure.Features.Classes.CQRS.Query
{
    public class GetActivitiesByClassQueryHandler : IRequestHandler<GetActivitiesByClassQuery, CommitResults<ClassActivityResponse>>
    {
        private readonly TrackerDbContext _dbContext;
        public GetActivitiesByClassQueryHandler(TrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<CommitResults<ClassActivityResponse>> Handle(GetActivitiesByClassQuery request, CancellationToken cancellationToken)
        {
            TeacherClass? teacherClass = await _dbContext.Set<TeacherClass>()
                .Where(a => a.IsActive && a.Id.Equals(request.ClassId))
                .Include(a => a.TeacherAssignments)
                .Include(a => a.TeacherQuizs)
                .SingleOrDefaultAsync(cancellationToken);

            if (teacherClass == null)
            {
                return new CommitResults<ClassActivityResponse>
                {
                    ResultType = ResultType.NotFound
                };
            }

            IEnumerable<TeacherAssignmentActivityTracker> assignmentActivityTrackers = await _dbContext.Set<TeacherAssignmentActivityTracker>()
                .Where(a => a.ActivityStatus == ActivityStatus.Finished && a.ClassId.Equals(request.ClassId))
                .ToListAsync(cancellationToken);


            IEnumerable<TeacherQuizActivityTracker> quizActivityTrackers = await _dbContext.Set<TeacherQuizActivityTracker>()
                .Where(a => a.ActivityStatus == ActivityStatus.Finished && a.ClassId.Equals(request.ClassId))
                .ToListAsync(cancellationToken);


            IEnumerable<ClassActivityResponse> Mapper()
            {
                foreach (TeacherQuiz teacherQuiz in teacherClass.TeacherQuizs)
                {
                    yield return new ClassActivityResponse
                    {
                        EndDate = teacherQuiz.EndDate,
                        Id = teacherQuiz.Id,
                        StartDate = teacherQuiz.CreatedOn.GetValueOrDefault(),
                        Title = teacherQuiz.Title,
                        TypeValue = 1,
                        TypeName = "Quiz",
                        EntrolledCounter = quizActivityTrackers.Where(a => a.TeacherQuizId.Equals(teacherQuiz.Id)).Count()
                    };
                }
                foreach (TeacherAssignment teacherAssignment in teacherClass.TeacherAssignments)
                {
                    yield return new ClassActivityResponse
                    {
                        EndDate = teacherAssignment.EndDate,
                        Id = teacherAssignment.Id,
                        StartDate = teacherAssignment.CreatedOn.GetValueOrDefault(),
                        Title = teacherAssignment.Title,
                        TypeValue = 2,
                        TypeName = "Assignment",
                        EntrolledCounter = assignmentActivityTrackers.Where(a => a.TeacherAssignmentId.Equals(teacherAssignment.Id)).Count()
                    };
                }
                yield break;
            }

            return new CommitResults<ClassActivityResponse>
            {
                ResultType = ResultType.Ok,
                Value = Mapper()
            };
        }
    }
}
