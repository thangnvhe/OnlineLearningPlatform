using OnlineLearningPlatform.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Domain.Entities
{
    public class ExamQuestion 
    {
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public int OrderIndex { get; set; }
        [ForeignKey("ExamId")]
        public Exam? Exam { get; set; }
        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }
    }
}
