namespace OnlineLearningPlatform.Domain.Abstract
{
    public interface IEventPublisher
    {
        // Ràng buộc where T : class là bắt buộc vì thư viện Message Broker nào cũng nhận payload là class/record
        Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class;
    }
}
