using OnlineLearningPlatform.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Domain.Entities
{
    public class StudentAnswer : BaseEntity<int>
    {
        public int ExamAttemptId { get; set; }
        public int QuestionId { get; set; }
        public int? SelectedOptionId { get; set; }
        public string? ContentAnswer { get; set; }
        [ForeignKey("ExamAttemptId")]
        public ExamAttempt? ExamAttempt { get; set; }
        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }
        [ForeignKey("SelectedOptionId")]
        public QuestionOption? SelectedOption { get; set; }
    }
}
