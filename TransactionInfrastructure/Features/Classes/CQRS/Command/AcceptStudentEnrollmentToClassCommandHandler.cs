using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Classes.CQRS.Command;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Invitation;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.TeacherClasses;

namespace TransactionInfrastructure.Features.Classes.CQRS.Command;

public class AcceptStudentEnrollmentToClassCommandHandler : IRequestHandler<AcceptStudentEnrollmentToClassCommand, CommitResult>
{
    private readonly TrackerDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public AcceptStudentEnrollmentToClassCommandHandler(TrackerDbContext dbContext,
                                                        IWebHostEnvironment configuration,
                                                        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }
    public async Task<CommitResult> Handle(AcceptStudentEnrollmentToClassCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.TeacherClass? teacherClass = await _dbContext.Set<DomainEntities.TeacherClass>()
                                                                    .Include(a => a.StudentEnrolls)
                                                                    .SingleOrDefaultAsync(a => a.Id.Equals(request.AddStudentToClassRequest.ClassId), cancellationToken);

        if (teacherClass == null)
        {
            return new CommitResult
            {
                ResultType = ResultType.NotFound,
                ErrorCode = "X0000",
                ErrorMessage = _resourceJsonManager["X0001"]
            };
        }

        teacherClass.StudentEnrolls.Add(new DomainEntities.StudentEnrollClass
        {
            ClassId = request.AddStudentToClassRequest.ClassId,
            StudentId = request.AddStudentToClassRequest.StudentId,
            IsActive = true
        });

        Invitation? invitation = await _dbContext.Set<Invitation>().SingleOrDefaultAsync(a => a.Id.Equals(request.AddStudentToClassRequest.InvitationId), cancellationToken);

        if (invitation == null)
        {
            return new CommitResult
            {
                ResultType = ResultType.NotFound,
                ErrorCode = "X0000",
                ErrorMessage = _resourceJsonManager["X0001"]
            };
        }
        invitation.IsActive = false;
        _dbContext.Set<Invitation>().Update(invitation);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}
