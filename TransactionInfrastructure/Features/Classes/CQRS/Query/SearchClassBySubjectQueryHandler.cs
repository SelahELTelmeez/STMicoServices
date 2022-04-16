using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Classes.CQRS.Query;
using TransactionDomain.Features.Classes.DTO.Query;
using TransactionDomain.Features.Shared.DTO;
using TransactionEntites.Entities;
using TransactionEntites.Entities.TeacherClasses;
using TransactionInfrastructure.HttpClients;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.Classes.CQRS.Query;
public class SearchClassBySubjectQueryHandler : IRequestHandler<SearchClassBySubjectQuery, CommitResults<ClassResponse>>
{
    private readonly TrackerDbContext _dbContext;
    private readonly IdentityClient _identityClient;
    private readonly Guid? _userId;

    public SearchClassBySubjectQueryHandler(TrackerDbContext dbContext, IdentityClient identityClient, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _identityClient = identityClient;
        _userId = httpContextAccessor.GetIdentityUserId();
    }
    public async Task<CommitResults<ClassResponse>> Handle(SearchClassBySubjectQuery request, CancellationToken cancellationToken)
    {

        IEnumerable<TeacherClass> teacherClasses = await _dbContext.Set<TeacherClass>()
                                                                   .Where(a => a.SubjectId.Equals(request.SubjectId) && a.IsActive)
                                                                   .Include(a => a.StudentEnrolls)
                                                                   .ToListAsync(cancellationToken);

        CommitResults<LimitedProfileResponse>? limitedProfiles = await _identityClient.GetIdentityLimitedProfilesAsync(teacherClasses.Select(a => a.TeacherId), cancellationToken);

        if (!limitedProfiles.IsSuccess)
        {
            return limitedProfiles.Adapt<CommitResults<ClassResponse>>();
        }

        IEnumerable<ClassResponse> Mapper()
        {
            foreach (TeacherClass teacherClass in teacherClasses)
            {
                LimitedProfileResponse? profileResponse = limitedProfiles?.Value?.SingleOrDefault(a => a.UserId.Equals(teacherClass.TeacherId));
                yield return new ClassResponse
                {
                    ClassId = teacherClass.Id,
                    Description = teacherClass.Description,
                    Name = teacherClass.Name,
                    SubjectId = teacherClass.SubjectId,
                    TeacherId = teacherClass.TeacherId,
                    TeacherName = profileResponse?.FullName,
                    AvatarUrl = profileResponse.AvatarImage,
                    IsEntrolled = teacherClass.StudentEnrolls.Any(a => a.StudentId.Equals(_userId)),
                };
            }
            yield break;
        }

        return new CommitResults<ClassResponse>
        {
            ResultType = ResultType.Ok,
            Value = Mapper()
        };
    }
}