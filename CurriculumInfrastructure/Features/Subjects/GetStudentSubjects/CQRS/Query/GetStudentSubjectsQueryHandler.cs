using CurriculumDomain.Features.Subjects.GetStudentSubjects.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetStudentSubjects.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumInfrastructure.HttpClients;
using Mapster;
using Microsoft.EntityFrameworkCore;
using CurriculumEntities = CurriculumEntites.Entities.Subjects;
namespace CurriculumInfrastructure.Features.Subjects.GetStudentSubjects.CQRS.Query;
public class GetStudentSubjectsQueryHandler : IRequestHandler<GetStudentSubjectsQuery, CommitResults<IdnentitySubjectResponse>>
{
    private readonly IdentityClient _IdentityClient;
    private readonly CurriculumDbContext _dbContext;

    public GetStudentSubjectsQueryHandler(IdentityClient identityClient, CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
        _IdentityClient = identityClient;
    }

    public async Task<CommitResults<IdnentitySubjectResponse>> Handle(GetStudentSubjectsQuery request, CancellationToken cancellationToken)
    {
        //========Calling Identity Micro-service to get the current grade of the user.==============

        CommitResult<int>? commitResult = await _IdentityClient.GetStudentGradesAsync(request.StudentId, cancellationToken);

        if (!commitResult.IsSuccess)
        {
            return commitResult.Adapt<CommitResults<IdnentitySubjectResponse>>();
        }
        //==================get response==================
        return new CommitResults<IdnentitySubjectResponse>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<CurriculumEntities.Subject>().Where(a => a.Grade == commitResult.Value && a.IsAppShow == true).ProjectToType<IdnentitySubjectResponse>().ToListAsync(cancellationToken)
        };
    }
}