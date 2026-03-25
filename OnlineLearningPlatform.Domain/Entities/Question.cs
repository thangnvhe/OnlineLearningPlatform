using OnlineLearningPlatform.Domain.Entities.Base;
using OnlineLearningPlatform.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Domain.Entities
{
    public class Question : BaseEntity<int>
    {
        public string Content { get; set; }
        public QuestionTypeEnum QuestionType { get; set; }
        public QuestionLevelEnum QuestionLevel { get; set; }
        public double Point { get; set; }
        public int? AudioId { get; set; }
        public bool IsActive { get; set; } = true;
        [ForeignKey("AudioId")]
        public AudioMedia? Audio { get; set; }
        public ICollection<QuestionOption>? Options { get; set; }
        public ICollection<StudentAnswer>? StudentAnswers { get; set; }
        public ICollection<ExamQuestion>? ExamQuestions { get; set; }
    }
}
