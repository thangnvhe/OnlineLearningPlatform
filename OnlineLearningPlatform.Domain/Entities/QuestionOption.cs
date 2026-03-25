using OnlineLearningPlatform.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Domain.Entities
{
    public class QuestionOption : BaseEntity<int>
    {
        public int QuestionId { get; set; }
        public string OptionText { get; set; }
        public bool? IsCorrect { get; set; }
        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }
    }
}
