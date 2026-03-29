using MassTransit;
using OnlineLearningPlatform.Domain.Abstract;

namespace OnlineLearningPlatform.Infrastruture.Services
{
    public class MassTransitEventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        // Bơm IPublishEndpoint của MassTransit vào đây
        public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : class
        {
            // Ủy quyền việc gửi tin nhắn cho MassTransit lo
            return _publishEndpoint.Publish(@event, ct);
        }
    }
}
