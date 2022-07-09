using ChatDomain.Features.CQRS.Query;
using ChatDomain.Features.DTO;
using ChatEntities;
using ChatEntities.Entities;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedModule.Extensions;

namespace ChatInfrastructure.Features.CQRS.Query
{
    public class LoadMessagesP2PQueryHandler : IRequestHandler<LoadMessagesP2PQuery, ICommitResults<ServerMessage>>
    {
        private readonly ChatDbContext _dbContext;
        private readonly Guid? _userId;

        public LoadMessagesP2PQueryHandler(ChatDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
        }
        public async Task<ICommitResults<ServerMessage>> Handle(LoadMessagesP2PQuery request, CancellationToken cancellationToken)
        {
            return ResultType.Ok.GetValueCommitResults(await _dbContext.Set<Message>()
                                                                       .Where(a => a.FromId.Equals(_userId)
                                                                                   && a.ToId.Equals(request.PeerId)
                                                                                   || a.FromId.Equals(request.PeerId)
                                                                                   && a.ToId.Equals(_userId))
                                                                       .OrderBy(a => a.Time)
                                                                       .ProjectToType<ServerMessage>()
                                                                       .ToListAsync(cancellationToken));
        }
    }
}
