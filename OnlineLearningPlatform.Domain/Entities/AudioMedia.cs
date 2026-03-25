using OnlineLearningPlatform.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.Domain.Entities
{
    public class AudioMedia : BaseEntity<int>
    {
        public string Title { get; set; }
        public string AudioUrl { get; set; }
        public string? Transcript { get; set; }
    }
}
