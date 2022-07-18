using NotifierDomain.Features.CQRS.Query;
using NotifierDomain.Models;
using NotifierDomain.Services;

namespace NotifierInfrastructure.Features.CQRS.Query
{
    public class SendFCMQueryHandler : IRequestHandler<SendFCMQuery, ICommitResult>
    {
        private readonly INotificationService _notification;
        private readonly IHttpClientFactory _httpClientFactory;
        public SendFCMQueryHandler(IHttpClientFactory httpClientFactory, INotificationService notification)
        {
            _httpClientFactory = httpClientFactory;
            _notification = notification;
        }
        public async Task<ICommitResult> Handle(SendFCMQuery request, CancellationToken cancellationToken)
        {
            bool result = await _notification.PushNotificationAsync(_httpClientFactory.CreateClient("FCMClient"), new NotificationModel
            {
                Token = request.Token,
                Type = 0,
                Title = "Test",
                Body = "This is a test FCM Message from the backend"

            }, cancellationToken);

            if (result == true)
            {
                return ResultType.Ok.GetCommitResult();
            }
            else
            {
                return ResultType.Invalid.GetCommitResult();
            }
        }
    }
}
