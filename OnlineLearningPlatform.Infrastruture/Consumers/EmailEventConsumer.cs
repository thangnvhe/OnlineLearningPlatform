using MassTransit;
using OnlineLearningPlatform.Application.DTOs.Events;
using OnlineLearningPlatform.Domain.Abstract;

namespace OnlineLearningPlatform.Infrastruture.Consumers
{
    public class EmailEventConsumer : IConsumer<SendEmailEvent>
    {
        private readonly IEmailService _emailService;

        public EmailEventConsumer(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Consume(ConsumeContext<SendEmailEvent> context)
        {
            var data = context.Message;

            await _emailService.SendTemplateEmailAsync(
                data.To,
                data.Subject,
                data.TemplateName,
                data.Model,
                context.CancellationToken);
        }
    }
}
