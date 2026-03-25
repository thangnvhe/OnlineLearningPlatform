using OnlineLearningPlatform.Domain.Entities.Base;
using OnlineLearningPlatform.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearningPlatform.Domain.Entities
{
    public class Material : BaseEntity<int>
    {
        public int SessionId { get; set; }
        public string? Title { get; set; }
        public string? FileUrl { get; set; }
        public MaterialTypeEnum? MaterialType { get; set; }
        [ForeignKey("SessionId")]
        public Session? Session { get; set; }
    }
}
