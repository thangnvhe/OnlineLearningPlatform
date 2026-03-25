using OnlineLearningPlatform.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Domain.Entities
{
    public class Session : BaseEntity<int>
    {
        public int ClassId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int OrderIndex { get; set; }
        [ForeignKey("ClassId")]
        public Classroom? Classroom { get; set; }
        public ICollection<Material>? Materials { get; set; }
        public ICollection<Exam>? Exams { get; set; }
    }
}
