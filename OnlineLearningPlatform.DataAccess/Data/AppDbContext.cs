using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Domain.Entities;
using System;
using OnlineLearningPlatform.Domain.Setting;

namespace OnlineLearningPlatform.DataAccess.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid, IdentityUserClaim<Guid>,
        UserRole, IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamAttempt> ExamAttempts { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<CheatLog> CheatLogs { get; set; }
        public DbSet<AudioMedia> AudioMedias { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
       
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Additional configuration can be added here if needed
            builder.Entity<ApplicationUser>().ToTable("ApplicationUser");
            builder.Entity<IdentityRole<Guid>>().ToTable("ApplicationRole");
            builder.Entity<UserRole>().ToTable("UserRole");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaim");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogin");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaim");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserToken");

            var adminRoleId = Guid.Parse("D5F0E5D4-3B9B-4D1D-9B1D-1D1D1D1D1D1D");
            var teacherRoleId = Guid.Parse("E6A1B2C3-4D5E-6F7A-8B9C-0D1E2F3A4B5C");
            var studentRoleId = Guid.Parse("F7B2C3D4-5E6F-7A8B-9C0D-1E2F3A4B5C6D");

            builder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid>
                {
                    Id = adminRoleId,
                    Name = CommonConstant.Role_Admin,
                    NormalizedName = CommonConstant.Role_Admin.ToUpper(),
                    ConcurrencyStamp = adminRoleId.ToString()
                },
                new IdentityRole<Guid>
                {
                    Id = teacherRoleId,
                    Name = CommonConstant.Role_Teacher,
                    NormalizedName = CommonConstant.Role_Teacher.ToUpper(),
                    ConcurrencyStamp = teacherRoleId.ToString()
                },
                new IdentityRole<Guid>
                {
                    Id = studentRoleId,
                    Name = CommonConstant.Role_Student,
                    NormalizedName = CommonConstant.Role_Student.ToUpper(),
                    ConcurrencyStamp = studentRoleId.ToString()
                }
            );
            builder.Entity<UserRole>(userRole =>
            {
                // Set khóa chính kép
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                // Nối với bảng ApplicationUser
                userRole.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles) // (Nhớ thêm ICollection<UserRole> UserRoles vào ApplicationUser)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                // Nối với bảng ApplicationRole (IdentityRole mặc định)
                userRole.HasOne(ur => ur.Role)
                    .WithMany() // Để trống vì IdentityRole<Guid> không chứa collection UserRole
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });
            // Fluent relationship configuration
            // Classroom - Teacher (ApplicationUser)
            builder.Entity<Classroom>()
                .HasOne(c => c.Teacher)
                .WithMany(u => u.Classrooms)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Classroom - Sessions
            builder.Entity<Session>()
                .HasOne(s => s.Classroom)
                .WithMany(c => c.Sessions)
                .HasForeignKey(s => s.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // Session - Materials
            builder.Entity<Material>()
                .HasOne(m => m.Session)
                .WithMany(s => s.Materials)
                .HasForeignKey(m => m.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Classroom - Enrollment - Student
            builder.Entity<Enrollment>()
                .HasOne(e => e.Classroom)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Classroom - Exam
            builder.Entity<Exam>()
                .HasOne(ex => ex.Classroom)
                .WithMany(c => c.Exams)
                .HasForeignKey(ex => ex.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // Session - Exam (optional)
            builder.Entity<Exam>()
                .HasOne(ex => ex.Session)
                .WithMany(s => s.Exams)
                .HasForeignKey(ex => ex.SessionId)
                .OnDelete(DeleteBehavior.NoAction);

            // Exam - ExamAttempt - Student
            builder.Entity<ExamAttempt>()
                .HasOne(a => a.Exam)
                .WithMany(ex => ex.ExamAttempts)
                .HasForeignKey(a => a.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ExamAttempt>()
                .HasOne(a => a.Student)
                .WithMany(u => u.ExamAttempts)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamAttempt - StudentAnswer
            builder.Entity<StudentAnswer>()
                .HasOne(sa => sa.ExamAttempt)
                .WithMany(a => a.StudentAnswers)
                .HasForeignKey(sa => sa.ExamAttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            // Question - QuestionOption
            builder.Entity<QuestionOption>()
                .HasOne(qo => qo.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(qo => qo.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Question - AudioMedia (optional)
            builder.Entity<Question>()
                .HasOne(q => q.Audio)
                .WithMany()
                .HasForeignKey(q => q.AudioId)
                .OnDelete(DeleteBehavior.SetNull);

            // Question - StudentAnswer
            builder.Entity<StudentAnswer>()
                .HasOne(sa => sa.Question)
                .WithMany(q => q.StudentAnswers)
                .HasForeignKey(sa => sa.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // StudentAnswer - SelectedOption (optional)
            builder.Entity<StudentAnswer>()
                .HasOne(sa => sa.SelectedOption)
                .WithMany()
                .HasForeignKey(sa => sa.SelectedOptionId)
                .OnDelete(DeleteBehavior.SetNull);

            // CheatLog - ExamAttempt
            builder.Entity<CheatLog>()
                .HasOne(cl => cl.ExamAttempt)
                .WithMany(a => a.CheatLogs)
                .HasForeignKey(cl => cl.ExamAttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            // Exam - ExamQuestion many-to-many via join entity with composite key
            builder.Entity<ExamQuestion>()
                .HasKey(eq => new { eq.ExamId, eq.QuestionId });

            builder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Exam)
                .WithMany(e => e.ExamQuestions)
                .HasForeignKey(eq => eq.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Question)
                .WithMany(q => q.ExamQuestions)
                .HasForeignKey(eq => eq.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}