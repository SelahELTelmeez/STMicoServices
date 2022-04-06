﻿using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ResultHandler;
using TransactionDomain.Features.Activities.CQRS.Command;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Trackers;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.Activities.CQRS.Command;

public class InsertActivityCommandHandler : IRequestHandler<InsertActivityCommand, CommitResult<int>>
{
    private readonly StudentTrackerDbContext _dbContext;
    private readonly Guid? _userId;

    // private readonly JsonLocalizerManager _resourceJsonManager;
    //private readonly TokenHandlerManager _jwtAccessGenerator;
    //private readonly INotificationService _notificationService;

    public InsertActivityCommandHandler(StudentTrackerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        //_resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        //_jwtAccessGenerator = tokenHandlerManager;
        //_notificationService = notificationService;
    }

    public async Task<CommitResult<int>> Handle(InsertActivityCommand request, CancellationToken cancellationToken)
    {
        // =========== Check for the clip of this student existance first ================
        StudentActivityTracker? StudentActivityTrackerChecker = await _dbContext.Set<StudentActivityTracker>()
                                                                                .FirstOrDefaultAsync(a => a.StudentId.Equals(_userId) && a.ClipId.Equals(request.ActivityRequest.ClipId), cancellationToken);
        if (StudentActivityTrackerChecker != null)
        {
            return new CommitResult<int>
            {
                ResultType = ResultType.Ok,
                Value = StudentActivityTrackerChecker.Id
            };
        }
        // =========== insert student Activity ================
        EntityEntry<StudentActivityTracker> studentActivityTracker = _dbContext.Set<StudentActivityTracker>().Add(request.ActivityRequest.Adapt<StudentActivityTracker>());
        await _dbContext.SaveChangesAsync(cancellationToken);

        // =========== Get Response ActivityId ================
        return new CommitResult<int>
        {
            ResultType = ResultType.Ok,
            Value = studentActivityTracker.Entity.Id
        };
    }
}
