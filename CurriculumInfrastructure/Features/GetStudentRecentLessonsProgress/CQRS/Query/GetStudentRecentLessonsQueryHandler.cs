namespace CurriculumInfrastructure.Features.GetStudentRecentLessonsProgress.CQRS.Query;

//public class GetStudentRecentLessonsQueryHandler : IRequestHandler<GetStudentRecentLessonsProgressQuery, CommitResult<List<StudentRecentLessonProgress>>>
//{
//    private readonly CurriculumDbContext _dbContext;
//    private readonly Guid? _userId;
//    public GetStudentRecentLessonsQueryHandler(IHttpContextAccessor httpContextAccessor, CurriculumDbContext dbContext)
//    {
//        _dbContext = dbContext;
//        _userId = httpContextAccessor.GetIdentityUserId();
//    }
//    public async Task<CommitResult<List<StudentRecentLessonProgress>>> Handle(GetStudentRecentLessonsProgressQuery request, CancellationToken cancellationToken)
//    {
//        // Read all User's activity 
//        List<int> activityRecords = await _dbContext.Set<StudentActivityRecord>()
//                                                                      .Where(a => a.StudentId.Equals(_userId))
//                                                                      .OrderByDescending(a => a.CreatedOn)
//                                                                      .Include(a => a.LessonFK)
//                                                                      .GroupBy(a => a.Id)
//                                                                      .Select(a => a.Key)
//                                                                      .Take(2)
//                                                                      .ToListAsync(cancellationToken);


//        if (activityRecords.Count == 1)
//        {
//            StudentActivityRecord? firstActivityRecord = await _dbContext.Set<StudentActivityRecord>().Include(a => a.LessonFK).SingleOrDefaultAsync(a => a.Id.Equals(activityRecords[0]), cancellationToken);
//            return new CommitResult<List<StudentRecentLessonProgress>>
//            {
//                ResultType = ResultType.Ok,
//                Value = new List<StudentRecentLessonProgress>
//                {
//                   new StudentRecentLessonProgress
//                   {
//                        LessonName = firstActivityRecord.LessonFK.Name,
//                        LessonPoints = firstActivityRecord.LessonFK.Ponits,
//                        StudentPoints = await _dbContext.Set<StudentActivityRecord>().Where(a => a.LessonId == firstActivityRecord.LessonId).SumAsync(a => a.StudentPoints,cancellationToken)
//                   },
//                },
//            };
//        }
//        if (activityRecords.Count == 2)
//        {
//            StudentActivityRecord? firstActivityRecord = await _dbContext.Set<StudentActivityRecord>().Include(a => a.LessonFK).SingleOrDefaultAsync(a => a.Id.Equals(activityRecords[0]), cancellationToken: cancellationToken);
//            StudentActivityRecord? secondActivityRecord = await _dbContext.Set<StudentActivityRecord>().Include(a => a.LessonFK).SingleOrDefaultAsync(a => a.Id.Equals(activityRecords[1]), cancellationToken: cancellationToken);

//            return new CommitResult<List<StudentRecentLessonProgress>>
//            {
//                ResultType = ResultType.Ok,
//                Value = new List<StudentRecentLessonProgress>
//                {
//                   new StudentRecentLessonProgress
//                   {
//                        LessonName = firstActivityRecord.LessonFK.Name,
//                        LessonPoints = firstActivityRecord.LessonFK.Ponits,
//                        StudentPoints = await _dbContext.Set<StudentActivityRecord>().Where(a => a.LessonId == firstActivityRecord.LessonId).SumAsync(a => a.StudentPoints, cancellationToken)
//                   },
//                   new StudentRecentLessonProgress
//                   {
//                        LessonName = secondActivityRecord.LessonFK.Name,
//                        LessonPoints = secondActivityRecord.LessonFK.Ponits,
//                        StudentPoints = await _dbContext.Set<StudentActivityRecord>().Where(a => a.LessonId == secondActivityRecord.LessonId).SumAsync(a => a.StudentPoints,cancellationToken)
//                   }
//                },
//            };
//        }

//        return new CommitResult<List<StudentRecentLessonProgress>>
//        {
//            ResultType = ResultType.Ok,
//            Value = new List<StudentRecentLessonProgress>()
//        };
//    }
//}