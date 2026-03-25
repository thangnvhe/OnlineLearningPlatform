using OnlineLearningPlatform.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Domain.Entities
{
    public class Classroom : BaseEntity<int>
    {
        public string ClassName { get; set; }
        public string ClassCode { get; set; }
        public Guid TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public ApplicationUser? Teacher { get; set; }
        public ICollection<Session>? Sessions { get; set; } 
        public ICollection<Enrollment>? Enrollments { get; set; }
        public ICollection<Exam>? Exams { get; set; }   
    }
}
