using Microsoft.AspNetCore.Identity;
using OnlineLearningPlatform.Domain.Entities.Base;

namespace OnlineLearningPlatform.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>, IEntity<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly Dob { get; set; }
        public bool IsMale { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ImgAvatarUrl { get; set; }
        public string? RefresherToken { get; set; }
        public DateTime? RefresherTokenExpiry { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Classroom>? Classrooms { get; set; }
        public ICollection<ExamAttempt>? ExamAttempts { get; set; }
        public ICollection<Enrollment>? Enrollments { get; set; }
        public virtual ICollection<UserRole>? UserRoles { get; set; }
    }
}
