using OnlineLearningPlatform.Domain.Entities.Base;
using OnlineLearningPlatform.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Domain.Entities
{
      
    public class CheatLog : BaseEntity<int>
    {
        public int ExamAttemptId { get; set; }
        public ViolationTypeEnum ViolationType { get; set; }
        public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
        public string? Description { get; set; }
        [ForeignKey("ExamAttemptId")]
        public ExamAttempt? ExamAttempt { get; set; }
    }
}
