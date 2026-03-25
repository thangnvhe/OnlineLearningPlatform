using OnlineLearningPlatform.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Domain.Entities
{
    public class Enrollment : BaseEntity<int>
    {

        public int ClassId { get; set; }
        public Guid StudentId { get; set; }
        public int Status { get; set; }
        public int CreatedAt { get; set; }
        [ForeignKey("ClassId")]
        public Classroom? Classroom { get; set; }
        [ForeignKey("StudentId")]
        public ApplicationUser? Student { get; set; }
    }
}
