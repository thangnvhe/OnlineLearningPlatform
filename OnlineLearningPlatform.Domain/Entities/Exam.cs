using OnlineLearningPlatform.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Domain.Entities
{
    public class Exam : BaseEntity<int>
    {
        public int ClassId { get; set; }
        public int? SessionId { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public int MaxAttempts { get; set; }
        public string ExamType { get; set; }
        public bool PreventTabSwitch { get; set; }
        public bool ShuffleQuestions { get; set; }
        public bool RequireCamera { get; set; }

        [ForeignKey("ClassId")]
        public Classroom? Classroom { get; set; }
        [ForeignKey("SessionId")]  
        public Session? Session { get; set; }
        public ICollection<ExamAttempt>? ExamAttempts { get; set; }
        public ICollection<ExamQuestion>? ExamQuestions { get; set; }
    }
}
