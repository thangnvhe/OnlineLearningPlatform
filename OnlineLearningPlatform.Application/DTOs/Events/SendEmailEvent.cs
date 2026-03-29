namespace OnlineLearningPlatform.Application.DTOs.Events
{
    public record SendEmailEvent
    {
        public required string To { get; init; }
        public required string Subject { get; init; }
        public required string TemplateName { get; init; }
        public required Dictionary<string, string> Model { get; init; }
    }
}
