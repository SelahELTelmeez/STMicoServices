﻿using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherEntites.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;

public class GetEnrolleeDetailsQueryHandler : IRequestHandler<GetEnrolleeDetailsQuery, ICommitResult<EnrolleeDetailsResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly CurriculumClient _curriculumClient;
    private readonly IdentityClient _identityClient;
    public GetEnrolleeDetailsQueryHandler(TeacherDbContext dbContext, IdentityClient identityClient, CurriculumClient curriculumClient)
    {
        _dbContext = dbContext;
        _identityClient = identityClient;
        _curriculumClient = curriculumClient;
    }
    public async Task<ICommitResult<EnrolleeDetailsResponse>> Handle(GetEnrolleeDetailsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<ClassEnrollee> enrolleeClasses = await _dbContext.Set<ClassEnrollee>()
                                                                     .Where(a => a.StudentId.Equals(request.EnrolleeId) && a.IsActive)
                                                                     .Include(a => a.TeacherClassFK)
                                                                     .ToListAsync(cancellationToken);
        if (enrolleeClasses.Any())
        {
            ICommitResult<LimitedProfileResponse>? limitedProfile = await _identityClient.GetIdentityLimitedProfileAsync(request.EnrolleeId, cancellationToken);
            if (!limitedProfile.IsSuccess)
            {
                return limitedProfile.ResultType.GetValueCommitResult((EnrolleeDetailsResponse)null, limitedProfile.ErrorCode, limitedProfile.ErrorMessage);
            }
            ICommitResults<SubjectResponse>? subjects = await _curriculumClient.GetSubjectsDetailsAsync(enrolleeClasses.Select(a => a.TeacherClassFK).Select(a => a.SubjectId), cancellationToken);
            if (!subjects.IsSuccess)
            {
                return subjects.ResultType.GetValueCommitResult((EnrolleeDetailsResponse)null, subjects.ErrorCode, subjects.ErrorMessage);
            }

            IEnumerable<ClassBriefResponse> ClassMapper()
            {
                foreach (ClassEnrollee classDetails in enrolleeClasses)
                {
                    yield return new ClassBriefResponse
                    {
                        Id = classDetails.Id,
                        Name = classDetails.TeacherClassFK.Name
                    };
                }
                yield break;
            }

            IEnumerable<SubjectBriefResponse> SubjectBriefMapper()
            {

                foreach (SubjectResponse subject in subjects.Value)
                {
                    yield return new SubjectBriefResponse
                    {
                        Id = subject.Id,
                        Name = subject.ShortName,
                        InternalIcon = subject.InternalIcon,
                    };
                }
                yield break;
            }


            return ResultType.Ok.GetValueCommitResult(new EnrolleeDetailsResponse
            {
                EnrolleeName = limitedProfile.Value.FullName,
                Classes = ClassMapper(),
                Subjects = SubjectBriefMapper(),
                ClassCounter = enrolleeClasses.Count()
            });
        }
        else
        {
            return ResultType.Empty.GetValueCommitResult((EnrolleeDetailsResponse) null);
        }
    }
}
