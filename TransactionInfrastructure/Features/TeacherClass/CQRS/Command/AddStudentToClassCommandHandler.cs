using JsonLocalizer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.TeacherClass.CQRS.Command;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Invitation;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.TeacherClasses;

namespace TransactionInfrastructure.Features.TeacherClass.CQRS.Command
{
    public class AddStudentToClassCommandHandler : IRequestHandler<AddStudentToClassCommand, CommitResult>
    {
        private readonly TrackerDbContext _dbContext;
        private readonly Guid? _userId;
        private readonly JsonLocalizerManager _resourceJsonManager;

        public AddStudentToClassCommandHandler(TrackerDbContext dbContext, IHttpContextAccessor httpContextAccessor, JsonLocalizerManager jsonLocalizerManager)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
            _resourceJsonManager = jsonLocalizerManager;
        }
        public async Task<CommitResult> Handle(AddStudentToClassCommand request, CancellationToken cancellationToken)
        {
            DomainEntities.TeacherClass? teacherClass = await _dbContext.Set<DomainEntities.TeacherClass>()
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
                StudentId = _userId.GetValueOrDefault(),
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
}
