using OnlineLearningPlatform.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Domain.Entities
{
    public class ExamAttempt : BaseEntity<int>
    {
        public int ExamId { get; set; }
        public Guid StudentId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime SubmitTime { get; set; }
        public double? TotalScore { get; set; }
        public int AttemptNumber { get; set; }
        public string TeacherComment { get; set; }
        public bool IsGraded { get; set; }
        [ForeignKey("ExamId")]
        public Exam? Exam { get; set; }
        [ForeignKey("StudentId")]
        public ApplicationUser? Student { get; set; }
        public ICollection<StudentAnswer>? StudentAnswers { get; set; }
        public ICollection<CheatLog>? CheatLogs { get; set; }

    }
}
